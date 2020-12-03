// Copyright (c) 2020 - 2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*===================================================================================
	EnvironmentVersion.cs
====================================================================================*/

namespace Shaderc
{
    /// <summary>
    /// Target environment version
    /// </summary>
    public enum EnvironmentVersion : uint
    {
        Vulkan_1_0 = ((1u << 22)),

        Vulkan_1_1 = ((1u << 22) | (1 << 12)),

        Vulkan_1_2 = ((1u << 22) | (2 << 12)),

        OpenGL_4_5 = 450,

        WebGPU,
    }
}
