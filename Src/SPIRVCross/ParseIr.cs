// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;

namespace SharpSPIRVCross
{
    public sealed class ParseIr 
    {
        public readonly IntPtr Handle;

        internal ParseIr(IntPtr handle)
        {
            Handle = handle;
        }
    }
}
