// Copyright (c) 2020 - 2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*===================================================================================
	TargetEnvironment.cs
====================================================================================*/


namespace Shaderc
{
    public enum TargetEnvironment : byte
    {
        /// <summary>
        /// SPIR-V under Vulkan semantics
        /// </summary>
        Vulkan,


        /// <summary>
        /// SPIR-V under OpenGL semantics
        /// </summary>
        /// <remarks>
        /// NOTE: SPIR-V code generation is not supported for shaders under OpenGL
        /// compatibility profile.
        /// </remarks>
        OpenGL,


        /// <summary>
        /// SPIR-V under OpenGL semantics,
        /// including compatibility profile
        /// functions
        /// </summary>
        OpenGLCompat,


        /// <summary>
        /// SPIR-V under WebGPU semantics.
        /// </summary>
        WebGPU,


        /// <summary>
        /// SPIR-V under Vulkan semantics
        /// </summary>
        Default = Vulkan
    }
}
