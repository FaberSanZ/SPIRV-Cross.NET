// Copyright (c) 2020 - 2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*===================================================================================
	UniformKind.cs
====================================================================================*/

namespace Shaderc
{
    public enum UniformKind : byte
    {
        Image,

        Sampler,

        Texture,

        Buffer,

        StorageBuffer,

        UnorderedAccessView,
    }
}
