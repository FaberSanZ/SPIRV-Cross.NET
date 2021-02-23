// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using static SharpSPIRVCross.spvc;

namespace SharpSPIRVCross
{
    public sealed class CompilerOptions
    {
        internal readonly IntPtr Handle;

        public bool IsDirty { get; internal set; }

        internal CompilerOptions(IntPtr handle)
        {
            Handle = handle;
        }

        public void SetOption(CompilerOption option, bool value)
        {
            spvc_compiler_options_set_bool(Handle, option, value);
            IsDirty = true;
        }

        public void SetOption(CompilerOption option, uint value)
        {
            spvc_compiler_options_set_uint(Handle, option, value);
            IsDirty = true;
        }
    }
}
