// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

namespace SharpSPIRVCross
{
    public enum CaptureMode
    {
        /// <summary>
        /// The Parsed IR payload will be copied, and the handle can be reused to create other compiler instances.
        /// </summary>
        Copy = 0,

        /// <summary>
        /// The payload will now be owned by the compiler.
        /// <see cref="ParseIr"/> should now be considered a dead blob and must not be used further.
        /// This is optimal for performance and should be the go-to option.
        /// </summary>
        TakeOwnership = 1
    }
}
