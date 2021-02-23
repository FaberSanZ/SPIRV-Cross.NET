// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static SharpSPIRVCross.spvc;

namespace SharpSPIRVCross
{
    public sealed class ShaderResources
    {
        internal readonly IntPtr Handle;

        internal ShaderResources(IntPtr handle)
        {
            Handle = handle;
        }

        public ReflectedResource[] GetResources(ResourceType resourceType)
        {
            unsafe
            {
                ReflectedResource.__Native* resource_list_ptr;
                spvc_resources_get_resource_list_for_type(Handle, resourceType,
                    &resource_list_ptr,
                    out var resource_size).CheckError();

                var resourceList = new ReflectedResource[resource_size.ToInt32()];

                for (int i = 0; i < resourceList.Length; i++)
                {
                    resourceList[i] = new ReflectedResource
                    {
                        Id = resource_list_ptr[i].Id,
                        BaseTypeId = resource_list_ptr[i].base_type_id,
                        TypeId = resource_list_ptr[i].type_id,
                        Name = Marshal.PtrToStringAnsi(resource_list_ptr[i].name)
                    };
                }

                return resourceList;
            }
        }
    }
}
