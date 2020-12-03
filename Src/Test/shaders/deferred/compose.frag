#include "preamble.inc"

layout (set = 0, binding = 0) uniform UBO {
    mat4 projection;
    mat4 model;
    mat4 view;
    vec4 camPos;    
    float exposure;
    float gamma;
    float prefilteredCubeMipLevels;
    float scaleIBLAmbient;
} ubo;

layout (constant_id = 0) const uint NUM_LIGHTS = 1;

struct Light {
    vec4 position;
    vec4 color;
    mat4 mvp;
};

layout (set = 0, binding = 4) uniform UBOLights {
    Light lights[NUM_LIGHTS];
};

const float M_PI = 3.141592653589793;
const float c_MinRoughness = 0.04;

layout (input_attachment_index = 0, set = 1, binding = 0) uniform subpassInputMS samplerColorRough;
layout (input_attachment_index = 1, set = 1, binding = 1) uniform subpassInputMS samplerEmitMetal;
layout (input_attachment_index = 2, set = 1, binding = 2) uniform subpassInputMS samplerN_AO;
layout (input_attachment_index = 3, set = 1, binding = 3) uniform subpassInputMS samplerPos;

layout (set = 0, binding = 1) uniform samplerCube samplerIrradiance;
layout (set = 0, binding = 2) uniform samplerCube prefilteredMap;
layout (set = 0, binding = 3) uniform sampler2D samplerBRDFLUT;

layout (location = 0) in vec2 inUV;

layout (location = 0) out vec4 outColor;

#include "pbr.inc"

// Calculation of the lighting contribution from an optional Image Based Light source.
// Precomputed Environment Maps are required uniform inputs and are computed as outlined in [1].
// See our README.md on Environment Maps [3] for additional discussion.
vec3 getIBLContribution(PBRInfo pbrInputs, vec3 n, vec3 reflection)
{
    float lod = (pbrInputs.perceptualRoughness * ubo.prefilteredCubeMipLevels);
    // retrieve a scale and bias to F0. See [1], Figure 3
    vec3 brdf = (texture(samplerBRDFLUT, vec2(pbrInputs.NdotV, 1.0 - pbrInputs.perceptualRoughness))).rgb;
    vec3 diffuseLight = texture(samplerIrradiance, reflection).rgb;

    vec3 specularLight = textureLod(prefilteredMap, reflection, lod).rgb;

    vec3 diffuse = diffuseLight * pbrInputs.diffuseColor;
    vec3 specular = specularLight * (pbrInputs.specularColor * brdf.x + brdf.y);

    // For presentation, this allows us to disable IBL terms
    // For presentation, this allows us to disable IBL terms
    diffuse *= ubo.scaleIBLAmbient;
    specular *= ubo.scaleIBLAmbient;

    return diffuse + specular;
}

void main() 
{
    if (subpassLoad(samplerPos, gl_SampleID).a == 1.0f)
        discard;
    
    float perceptualRoughness = subpassLoad(samplerColorRough, gl_SampleID).a;
    float metallic = subpassLoad(samplerEmitMetal, gl_SampleID).a;
    vec3 diffuseColor;
    vec4 baseColor = vec4(subpassLoad(samplerColorRough, gl_SampleID).rgb, 1);    
    vec3 emissive = subpassLoad (samplerEmitMetal, gl_SampleID).rgb;        

    vec3 f0 = vec3(0.04);
    
    diffuseColor = baseColor.rgb * (vec3(1.0) - f0);
    diffuseColor *= 1.0 - metallic;
        
    float alphaRoughness = perceptualRoughness * perceptualRoughness;

    vec3 specularColor = mix(f0, baseColor.rgb, metallic);

    // Compute reflectance.
    float reflectance = max(max(specularColor.r, specularColor.g), specularColor.b);

    // For typical incident reflectance range (between 4% to 100%) set the grazing reflectance to 100% for typical fresnel effect.
    // For very low reflectance range on highly diffuse objects (below 4%), incrementally reduce grazing reflecance to 0%.
    float reflectance90 = clamp(reflectance * 25.0, 0.0, 1.0);
    vec3 specularEnvironmentR0 = specularColor.rgb;
    vec3 specularEnvironmentR90 = vec3(1.0) * reflectance90;

    vec3 n = subpassLoad(samplerN_AO, gl_SampleID).rgb;
    vec3 pos = subpassLoad(samplerPos, gl_SampleID).rgb;
    vec3 v = normalize(ubo.camPos.xyz - pos); // Vector from surface point to camera
    
    vec3 colors = vec3(0);
    vec3 lightTarget = vec3(0);
    
    for (int i=0; i<NUM_LIGHTS; i++) {
    
        vec3 l = normalize(lights[i].position.xyz-pos);     // Vector from surface point to light
        vec3 h = normalize(l+v);                        // Half vector between both l and v
        vec3 reflection = -normalize(reflect(v, n));
        reflection.y *= -1.0f;

        float NdotL = clamp(dot(n, l), 0.001, 1.0);
        float NdotV = clamp(abs(dot(n, v)), 0.001, 1.0);
        float NdotH = clamp(dot(n, h), 0.0, 1.0);
        float LdotH = clamp(dot(l, h), 0.0, 1.0);
        float VdotH = clamp(dot(v, h), 0.0, 1.0);

        PBRInfo pbrInputs = PBRInfo(
            NdotL,
            NdotV,
            NdotH,
            LdotH,
            VdotH,
            perceptualRoughness,
            metallic,
            specularEnvironmentR0,
            specularEnvironmentR90,
            alphaRoughness,
            diffuseColor,
            specularColor
        );

        // Calculate the shading terms for the microfacet specular shading model
        vec3 F = specularReflection(pbrInputs);
        float G = geometricOcclusion(pbrInputs);
        float D = microfacetDistribution(pbrInputs);

        // Calculation of analytical lighting contribution
        vec3 diffuseContrib = (1.0 - F) * diffuse(pbrInputs);
        vec3 specContrib = F * G * D / (4.0 * NdotL * NdotV);        
        // Obtain final intensity as reflectance (BRDF) scaled by the energy of the light (cosine law)
        vec3 color = NdotL * lights[i].color.rgb * (diffuseContrib + specContrib);
        // Calculate lighting contribution from image based lighting source (IBL)
        colors += (color + getIBLContribution(pbrInputs, n, reflection));
        
        
    }
    colors /= NUM_LIGHTS;
    
    
    const float u_OcclusionStrength = 1.0f;
    const float u_EmissiveFactor = 1.0f;
    
    //AO is in the alpha channel of the normalAttachment    
    colors = mix(colors, colors * subpassLoad(samplerN_AO, gl_SampleID).a, u_OcclusionStrength);
    colors += emissive;             
    
    outColor = vec4(colors, baseColor.a);       
}