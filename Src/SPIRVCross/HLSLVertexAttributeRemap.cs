// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

namespace SharpSPIRVCross
{
    public struct HLSLVertexAttributeRemap
    {
        public uint Location;
        public string Semantic;

        #region Marshal
        internal struct __Native
        {
            public uint Location;
            public IntPtr Semantic;
        }

        internal void __MarshalTo(ref __Native @ref)
        {
            @ref.Location = Location;
            @ref.Semantic = Marshal.StringToHGlobalAnsi(Semantic);
        }

        internal void __MarshalFree(ref __Native @ref)
        {
            Marshal.FreeHGlobal(@ref.Semantic);
        }
        #endregion
    }
}
