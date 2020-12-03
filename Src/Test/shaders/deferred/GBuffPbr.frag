#version 450
#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable

#define MANUAL_SRGB 0

struct Material {
    vec4 baseColorFactor;
    vec4 emissiveFactor;
    vec4 diffuseFactor;
    vec4 specularFactor;
    float workflow;
    uint tex0;
    uint tex1;
    float metallicFactor;   
    float roughnessFactor;  
    float alphaMask;    
    float alphaMaskCutoff;
    int pad0;
};

#include "GBuffPbrCommon.inc"
#include "tonemap.inc"

// Material bindings
layout (set = 2, binding = 0) uniform sampler2D colorMap;
layout (set = 2, binding = 1) uniform sampler2D physicalDescriptorMap;
layout (set = 2, binding = 2) uniform sampler2D normalMap;
layout (set = 2, binding = 3) uniform sampler2D aoMap;
layout (set = 2, binding = 4) uniform sampler2D emissiveMap;

// Find the normal for this fragment, pulling either from a predefined normal map
// or from the interpolated mesh normal and tangent attributes.
vec3 getNormal()
{
    vec3 tangentNormal;
    // Perturb normal, see http://www.thetenthplanet.de/archives/1180
    if ((materials[materialIdx].tex0 & MAP_NORMAL) == MAP_NORMAL)
        tangentNormal = texture(normalMap, inUV0).xyz * 2.0 - 1.0;
    else if ((materials[materialIdx].tex1 & MAP_NORMAL) == MAP_NORMAL)
        tangentNormal = texture(normalMap, inUV1).xyz * 2.0 - 1.0;
    else
        return normalize(inNormal);
        
    vec3 q1 = dFdx(inWorldPos);
    vec3 q2 = dFdy(inWorldPos);
    vec2 st1 = dFdx(inUV0);
    vec2 st2 = dFdy(inUV0);

    vec3 N = normalize(inNormal);
    vec3 T = normalize(q1 * st2.t - q2 * st1.t);
    vec3 B = -normalize(cross(N, T));
    mat3 TBN = mat3(T, B, N);

    return normalize(TBN * tangentNormal);
}

void main() 
{
    float perceptualRoughness;
    float metallic;    
    vec4 baseColor;
    vec3 emissive = vec3(0);    
    
    baseColor = materials[materialIdx].baseColorFactor;
    
    if (materials[materialIdx].workflow == PBR_WORKFLOW_METALLIC_ROUGHNESS) {
        perceptualRoughness = materials[materialIdx].roughnessFactor;
        metallic = materials[materialIdx].metallicFactor;        
        // Roughness is stored in the 'g' channel, metallic is stored in the 'b' channel.
        // This layout intentionally reserves the 'r' channel for (optional) occlusion map data
        if ((materials[materialIdx].tex0 & MAP_METALROUGHNESS) == MAP_METALROUGHNESS){
            perceptualRoughness *= texture(physicalDescriptorMap, inUV0).g;
            metallic *= texture(physicalDescriptorMap, inUV0).b;
        }else if ((materials[materialIdx].tex1 & MAP_METALROUGHNESS) == MAP_METALROUGHNESS){
            perceptualRoughness *= texture(physicalDescriptorMap, inUV1).g;
            metallic *= texture(physicalDescriptorMap, inUV1).b;
        }               
        perceptualRoughness = clamp(perceptualRoughness, c_MinRoughness, 1.0);
        metallic = clamp(metallic, 0.0, 1.0);        

        // The albedo may be defined from a base texture or a flat color
        if ((materials[materialIdx].tex0 & MAP_COLOR) == MAP_COLOR)        
            baseColor *= SRGBtoLINEAR(texture(colorMap, inUV0));
        else if ((materials[materialIdx].tex1 & MAP_COLOR) == MAP_COLOR)
            baseColor *= SRGBtoLINEAR(texture(colorMap, inUV1));        
    }
    
    if (materials[materialIdx].alphaMask == 1.0f) {            
        if (baseColor.a < materials[materialIdx].alphaMaskCutoff) 
            discard;        
    }

    if (materials[materialIdx].workflow == PBR_WORKFLOW_SPECULAR_GLOSINESS) {
        // Values from specular glossiness workflow are converted to metallic roughness
        if ((materials[materialIdx].tex0 & MAP_METALROUGHNESS) == MAP_METALROUGHNESS)
            perceptualRoughness = 1.0 - texture(physicalDescriptorMap, inUV0).a;            
        else if ((materials[materialIdx].tex1 & MAP_METALROUGHNESS) == MAP_METALROUGHNESS)
            perceptualRoughness = 1.0 - texture(physicalDescriptorMap, inUV1).a;            
        else
            perceptualRoughness = 0.0;

        const float epsilon = 1e-6;

        vec4 diffuse = SRGBtoLINEAR(texture(colorMap, inUV0));
        vec3 specular = SRGBtoLINEAR(texture(physicalDescriptorMap, inUV0)).rgb;

        float maxSpecular = max(max(specular.r, specular.g), specular.b);

        // Convert metallic value from specular glossiness inputs
        metallic = convertMetallic(diffuse.rgb, specular, maxSpecular);

        vec3 baseColorDiffusePart = diffuse.rgb * ((1.0 - maxSpecular) / (1 - c_MinRoughness) / max(1 - metallic, epsilon)) * materials[materialIdx].diffuseFactor.rgb;
        vec3 baseColorSpecularPart = specular - (vec3(c_MinRoughness) * (1 - metallic) * (1 / max(metallic, epsilon))) * materials[materialIdx].specularFactor.rgb;
        baseColor = vec4(mix(baseColorDiffusePart, baseColorSpecularPart, metallic * metallic), diffuse.a);

    }        
    
    const float u_OcclusionStrength = 1.0f;
    const float u_EmissiveFactor = 1.0f;
    float ao = 1.0f;
    
    if ((materials[materialIdx].tex0 & MAP_EMISSIVE) == MAP_EMISSIVE)    
        emissive = SRGBtoLINEAR(texture(emissiveMap, inUV0)).rgb * u_EmissiveFactor;
    else if ((materials[materialIdx].tex1 & MAP_EMISSIVE) == MAP_EMISSIVE)    
        emissive = SRGBtoLINEAR(texture(emissiveMap, inUV1)).rgb * u_EmissiveFactor;
    
    if ((materials[materialIdx].tex0 & MAP_AO) == MAP_AO)
        ao = texture(aoMap, inUV0).r;
    else if ((materials[materialIdx].tex1 & MAP_AO) == MAP_AO)
        ao = texture(aoMap, inUV1).r;
        
    vec3 n = getNormal();    
    vec3 p = inWorldPos;
    
    outColorRough = vec4 (baseColor.rgb, perceptualRoughness);
    outEmitMetal = vec4 (emissive, metallic);
    outN_AO = vec4 (n, ao);
    outPos = vec4 (p, linearDepth(gl_FragCoord.z));
}