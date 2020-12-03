#version 450

#extension GL_ARB_separate_shader_objects : enable
#extension GL_ARB_shading_language_420pack : enable

layout (location = 0) in vec3 inPos;
layout (location = 1) in vec3 inColor;

layout (location = 0) out vec3 outColor;

layout(push_constant) uniform PushConsts {
    mat4 projection;
    mat4 view;
};

out gl_PerVertex 
{
    vec4 gl_Position;   
};

void main() 
{
    outColor = inColor;    
	//gl_Position = projection * vec4 ((view * vec4(inPos.xyz, 0.0)).xyz, 1);
	gl_Position = projection * view * vec4(inPos.xyz, 1);
}
