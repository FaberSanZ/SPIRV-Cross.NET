#version 450

#extension GL_ARB_separate_shader_objects : enable


layout(binding = 1) uniform sampler2D texSampler;

layout (binding = 0) uniform UBO 
{
	mat4 modelMatrix;
	mat4 viewMatrix;
	mat4 projectionMatrix;
} ubo;


layout (binding = 2) uniform DATA 
{
	mat4 model;
} data;

layout(location = 0) in vec3 fragColor;
layout(location = 1) in vec2 fragTexCoord;

layout(location = 0) out vec4 outColor;

void main() {
    outColor = texture(texSampler, fragTexCoord);
}




