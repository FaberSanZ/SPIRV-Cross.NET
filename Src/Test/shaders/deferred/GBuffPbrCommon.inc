layout (constant_id = 0) const float NEAR_PLANE = 0.1f;
layout (constant_id = 1) const float FAR_PLANE = 256.0f;
layout (constant_id = 2) const int MAT_COUNT = 1;

const float M_PI = 3.141592653589793;
const float c_MinRoughness = 0.04;

const float PBR_WORKFLOW_METALLIC_ROUGHNESS = 1.0;
const float PBR_WORKFLOW_SPECULAR_GLOSINESS = 2.0f;

const uint MAP_COLOR = 0x1;
const uint MAP_NORMAL = 0x2;
const uint MAP_AO = 0x4;
const uint MAP_METAL = 0x8;
const uint MAP_ROUGHNESS = 0x10;
const uint MAP_METALROUGHNESS = 0x20;
const uint MAP_EMISSIVE = 0x40;

layout (location = 0) in vec3 inWorldPos;
layout (location = 1) in vec3 inNormal;
layout (location = 2) in vec2 inUV0;
layout (location = 3) in vec2 inUV1;

layout (set = 0, binding = 5) uniform UBOMaterials {
    Material materials[MAT_COUNT];
};

layout (push_constant) uniform PushCsts {
    layout(offset = 64)
    int materialIdx;
};

layout (location = 0) out vec4 outColorRough;
layout (location = 1) out vec4 outEmitMetal;
layout (location = 2) out vec4 outN_AO;
layout (location = 3) out vec4 outPos;

// Gets metallic factor from specular glossiness workflow inputs 
float convertMetallic(vec3 diffuse, vec3 specular, float maxSpecular) {
    float perceivedDiffuse = sqrt(0.299 * diffuse.r * diffuse.r + 0.587 * diffuse.g * diffuse.g + 0.114 * diffuse.b * diffuse.b);
    float perceivedSpecular = sqrt(0.299 * specular.r * specular.r + 0.587 * specular.g * specular.g + 0.114 * specular.b * specular.b);
    if (perceivedSpecular < c_MinRoughness) {
        return 0.0;
    }
    float a = c_MinRoughness;
    float b = perceivedDiffuse * (1.0 - maxSpecular) / (1.0 - c_MinRoughness) + perceivedSpecular - 2.0 * c_MinRoughness;
    float c = c_MinRoughness - perceivedSpecular;
    float D = max(b * b - 4.0 * a * c, 0.0);
    return clamp((-b + sqrt(D)) / (2.0 * a), 0.0, 1.0);
}

float linearDepth(float depth)
{
    float z = depth * 2.0f - 1.0f; 
    return (2.0f * NEAR_PLANE * FAR_PLANE) / (FAR_PLANE + NEAR_PLANE - z * (FAR_PLANE - NEAR_PLANE));   
}