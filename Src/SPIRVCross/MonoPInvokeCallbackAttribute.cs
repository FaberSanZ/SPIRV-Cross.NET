// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;

namespace SharpSPIRVCross
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class MonoPInvokeCallbackAttribute : Attribute
    {
        public Type Type { get; }

        public MonoPInvokeCallbackAttribute(Type type)
        {
            Type = type;
        }
    }
}
