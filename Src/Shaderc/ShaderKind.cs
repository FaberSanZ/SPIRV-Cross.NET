// Copyright (c) 2020 - 2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*===================================================================================
	ShaderKind.cs
====================================================================================*/

namespace Shaderc
{
    public enum ShaderKind : byte
    {
        VertexShader,
        FragmentShader,
        ComputeShader,
        GeometryShader,
        TessControlShader,
        TessEvaluationShader,

        GlslVertexShader = VertexShader,
        GlslFragmentShader = FragmentShader,
        GlslComputeShader = ComputeShader,
        GlslGeometryShader = GeometryShader,
        GlslTessControlShader = TessControlShader,
        GlslTessEvaluationShader = TessEvaluationShader,

        GlslInferFromSource,
        GlslDefaultVertexShader,
        GlslDefaultFragmentShader,
        GlslDefaultComputeShader,
        GlslDefaultGeometryShader,
        GlslDefaultTessControlShader,
        GlslDefaultTessEvaluationShader,

        SpirvAssembly,
        RaygenShader,
        AnyhitShader,
        ClosesthitShader,
        MissShader,
        IntersectionShader,
        CallableShader,

        GlslRaygenShader = RaygenShader,
        GlslAnyhitShader = AnyhitShader,
        GlslClosesthitShader = ClosesthitShader,
        GlslMissShader = MissShader,
        GlslIntersectionShader = IntersectionShader,
        GlslCallableShader = CallableShader,

        GlslDefaultRaygenShader,
        GlslDefaultAnyhitShader,
        GlslDefaultClosesthitShader,
        GlslDefaultMissShader,
        GlslDefaultIntersectionShader,
        GlslDefaultCallableShader,
        TaskShader,
        MeshShader,
        GlslTaskShader = TaskShader,
        GlslMeshShader = MeshShader,
        GlslDefaultTaskShader,
        GlslDefaultMeshShader,
    }
}
