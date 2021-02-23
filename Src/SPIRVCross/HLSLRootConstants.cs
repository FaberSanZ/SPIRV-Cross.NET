// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

namespace SharpSPIRVCross
{
    public readonly struct HLSLRootConstants
    {
        public readonly uint Start;
        public readonly uint End;
        public readonly uint Binding;
        public readonly uint Space;

        public HLSLRootConstants(uint start, uint end, uint binding, uint space)
        {
            Start = start;
            End = end;
            Binding = binding;
            Space = space;
        }
    }
}
