// Copyright (c) 2020 - 2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*===================================================================================
	Status.cs
====================================================================================*/

namespace Shaderc
{
    public enum Status : byte
    {
        Success = 0,

        InvalidStage = 1,  

        CompilationError = 2,

        InternalError = 3, 

        NullResultObject = 4,

        InvalidAssembly = 5,

        ValidationError = 6,

        TransformationError = 7,

        ConfigurationError = 8,
    }
}
