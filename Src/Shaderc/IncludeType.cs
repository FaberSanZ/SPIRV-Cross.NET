// Copyright (c) 2020 - 2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*===================================================================================
	IncludeType.cs
====================================================================================*/

namespace Shaderc
{

    /// <summary>
    /// The kinds of include requests.
    /// </summary>
    public enum IncludeType
    {
        /// <summary>
        /// E.g. #include "source"
        /// </summary>
        Relative,

        /// <summary>
        /// E.g. #include &lt;source>
        /// </summary>
        Standard
    }
}
