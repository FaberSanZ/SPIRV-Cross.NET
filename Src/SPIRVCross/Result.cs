// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

namespace SharpSPIRVCross
{
    public enum Result
    {
        /// <summary>
        /// Success.
        /// </summary>
        Success = 0,

        /// <summary>
        /// The SPIR-V is invalid. Should have been caught by validation ideally.
        /// </summary>
        ErrorInvalidSPIRV = -1,

        /// <summary>
        /// The SPIR-V might be valid or invalid, but SPIRV-Cross currently cannot correctly translate this to your target language.
        /// </summary>
        ErrorUnsupportedSPIRV = -2,

        /// <summary>
        /// If for some reason we hit this, new or malloc failed.
        /// </summary>
        ErrorOutOfMemory = -3,

        /// <summary>
        /// Invalid API argument.
        /// </summary>
        ErrorInvalidArgument = -4,
    }

    public static class ResultExtensions
    {
        public static void CheckError(this Result result)
        {
            if (result < 0)
            {
                throw new SharpSPIRVCrossException(result);
            }
        }
    }
}
