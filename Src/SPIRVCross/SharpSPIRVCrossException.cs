// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;

namespace SharpSPIRVCross
{
    public sealed class SharpSPIRVCrossException : Exception
    {
        public readonly IntPtr _handle;

        public Result Result { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SharpSPIRVCrossException" /> class.
        /// </summary>
        /// <param name="result">The result code that caused this exception.</param>
        public SharpSPIRVCrossException(Result result)
            : base($"A SPIRV-Cross error of type [{result}] occurred.")
        {
            Result = result;
        }

        public SharpSPIRVCrossException()
        {
        }

        public SharpSPIRVCrossException(string message)
            : base(message)
        {
        }

        public SharpSPIRVCrossException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
