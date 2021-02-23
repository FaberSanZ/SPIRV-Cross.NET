// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

namespace SharpSPIRVCross
{
    public enum Backend
    {
        /// <summary>
        /// This backend can only perform reflection, no compiler options are supported.
        /// </summary>
        None = 0,

        /// <summary>
        /// GLSL compiler.
        /// </summary>
        GLSL = 1,

        /// <summary>
        /// HLSL compiler.
        /// </summary>
        HLSL = 2,

        /// <summary>
        /// Metal compiler.
        /// </summary>
        MSL = 3,

        /// <summary>
        /// C++ compiler.
        /// </summary>
        CPP = 4,

        /// <summary>
        /// JSON reflection compiler.
        /// </summary>
        JSON = 5,
    }
}
