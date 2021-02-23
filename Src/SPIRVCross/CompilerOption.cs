// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using static SharpSPIRVCross.spvc;

namespace SharpSPIRVCross
{
    public enum CompilerOption
    {
        Unknown = 0,
        ForceTemporary = 1 | 0x1000000,
        FlattenMultidimensionalArrays= 2 | 0x1000000,
        FixupDepthConvention = 3 | 0x1000000,
        FlipVertexY = 4 | 0x1000000,

        GLSL_SupportNonZeroBaseInstance = 5 | 0x2000000,
        GLSL_SeparateShaderObjects = 6 | 0x2000000,
        GLSL_Enable420PackExtension = 7 | 0x2000000,
        GLSL_Version = 8 | 0x2000000,
        GLSL_ES = 9 | 0x2000000,
        GLSL_VulkanSemantics = 10 | 0x2000000,
        GLSL_ES_DefaultFloatPrecisionHighP = 11 | 0x2000000,
        GLSL_ES_DefaultIntPrecisionHighP = 12 | 0x2000000,
        GLSL_EmitPushConstantAsUniformBuffer = 33 | 0x2000000,
        SPVC_COMPILER_OPTION_GLSL_EMIT_UNIFORM_BUFFER_AS_PLAIN_UNIFORMS = 35 | 0x2000000,

        HLSL_ShaderModel = 13 | 0x4000000,
        HLSL_PointSizeCompat = 14 | 0x4000000,
        HLSL_PointCoordCompat = 15 | 0x4000000,
        HLSL_SupportNonZeroBaseVertexInstance = 16 | 0x4000000,

        MSL_Version = 17 | 0x8000000,
        MSL_TexelBufferTextureWidth = 18 | 0x8000000,
        MSL_AUX_BUFFER_INDEX = 19 | 0x8000000,
        MSL_IndirectParamsBufferIndex = 20 | 0x8000000,
        MSL_ShaderOutputBufferIndex = 21 | 0x8000000,
        MSL_ShaderPatchOutputBufferIndex = 22 | 0x8000000,
        MSL_ShaderTessFactorOutputBufferIndex = 23 | 0x8000000,
        MSL_ShaderInputWorkgroupIndex = 24 | 0x8000000,
        MSL_EnablePointSizeBuiltIn = 25 | 0x8000000,
        MSL_DisableRasterization = 26 | 0x8000000,
        MSL_CaptureOutputToBuffer = 27 | 0x8000000,
        MSL_SwizzleTextureSamples = 28 | 0x8000000,
        MSL_PadFragmentOutputComponents = 29 | 0x8000000,
        MSL_TessDomainOriginLowerLeft = 30 | 0x8000000,
        MSL_Platform = 31 | 0x8000000,
        MSL_ArgumentBuffers = 32 | 0x8000000,
        MSL_TextureBufferNative = 34 | 0x8000000,
    }
}
