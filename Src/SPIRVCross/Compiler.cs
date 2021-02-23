// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static SharpSPIRVCross.spvc;

namespace SharpSPIRVCross
{
    public sealed class Compiler
    {
        internal readonly IntPtr Handle;

        public readonly CompilerOptions Options;

        public readonly SpvExecutionModel ExecutionModel;

        internal Compiler(IntPtr handle)
        {
            Handle = handle;

            spvc_compiler_create_compiler_options(handle, out var optionsPtr).CheckError();
            Options = new CompilerOptions(optionsPtr);
            ExecutionModel = spvc_compiler_get_execution_model(handle);
        }

        public string Compile()
        {
            // Apply options.
            if (Options.IsDirty)
            {
                spvc_compiler_install_compiler_options(Handle, Options.Handle);
                Options.IsDirty = false;
            }

            var result = spvc_compiler_compile(Handle, out var source);
            result.CheckError();
            return Marshal.PtrToStringAnsi(source);
        }

        public void AddHeaderLine(string line)
        {
            spvc_compiler_add_header_line(Handle, line).CheckError();
        }

        public void RequireExtension(string extension)
        {
            spvc_compiler_require_extension(Handle, extension).CheckError();
        }

        public void FlattenBufferBlock(uint id)
        {
            spvc_compiler_flatten_buffer_block(Handle, id).CheckError();
        }

        public unsafe void HLSLSetRootConstantsLayout(params HLSLRootConstants[] constants)
        {
            spvc_compiler_hlsl_set_root_constants_layout(
                Handle,
                (HLSLRootConstants*)Unsafe.AsPointer(ref constants),
                new IntPtr(constants.Length)
                ).CheckError();
        }

        public unsafe void HLSLAddVertexAttributeRemap(params HLSLVertexAttributeRemap[] remaps)
        {
            var nativeRemaps = stackalloc HLSLVertexAttributeRemap.__Native[remaps.Length];
            for (var i = 0; i < remaps.Length; i++)
            {
                remaps[i].__MarshalTo(ref nativeRemaps[i]);
            }

            spvc_compiler_hlsl_add_vertex_attribute_remap(Handle,
                nativeRemaps,
                new IntPtr(remaps.Length)).CheckError();

            for (var i = 0; i < remaps.Length; i++)
            {
                remaps[i].__MarshalFree(ref nativeRemaps[i]);
            }
        }

        public uint HLSLRemapNumWorkgroupsBuiltin()
        {
            return spvc_compiler_hlsl_remap_num_workgroups_builtin(Handle);
        }

        public ShaderResources CreateShaderResources()
        {
            spvc_compiler_create_shader_resources(Handle, out var resourcesPtr).CheckError();
            return new ShaderResources(resourcesPtr);
        }

        public void SetDecoration(uint id, SpvDecoration decoration, uint argument)
        {
            spvc_compiler_set_decoration(Handle, id, decoration, argument);
        }

        public void SetDecoration(uint id, SpvDecoration decoration, string argument)
        {
            spvc_compiler_set_decoration_string(Handle, id, decoration, argument);
        }

        public void SetName(uint id, string argument)
        {
            spvc_compiler_set_name(Handle, id, argument);
        }

        public void SetMemberDecoration(uint id, uint memberIndex, SpvDecoration decoration, uint argument)
        {
            spvc_compiler_set_member_decoration(Handle, id, memberIndex, decoration, argument);
        }

        public void SetMemberDecoration(uint id, uint memberIndex, SpvDecoration decoration, string argument)
        {
            spvc_compiler_set_member_decoration_string(Handle, id, memberIndex, decoration, argument);
        }

        public void SetMemberName(uint id, uint memberIndex, string argument)
        {
            spvc_compiler_set_member_name(Handle, id, memberIndex, argument);
        }

        public void UnsetDecoration(uint id, SpvDecoration decoration)
        {
            spvc_compiler_unset_decoration(Handle, id, decoration);
        }

        public void UnsetMemberDecoration(uint id, uint memberIndex, SpvDecoration decoration)
        {
            spvc_compiler_unset_member_decoration(Handle, id, memberIndex, decoration);
        }

        public bool HasDecoration(uint id, SpvDecoration decoration)
        {
            return spvc_compiler_has_decoration(Handle, id, decoration);
        }

        public bool HasMemberDecoration(uint id, uint memberIndex, SpvDecoration decoration)
        {
            return spvc_compiler_has_member_decoration(Handle, id, memberIndex, decoration);
        }

        public string GetName(uint id)
        {
            return Marshal.PtrToStringAnsi(spvc_compiler_get_name(Handle, id));
        }

        public uint GetDecoration(uint id, SpvDecoration decoration)
        {
            return spvc_compiler_get_decoration(Handle, id, decoration);
        }

        public string GetDecorationString(uint id, SpvDecoration decoration)
        {
            return Marshal.PtrToStringAnsi(spvc_compiler_get_decoration_string(Handle, id, decoration));
        }

        public uint GetMemberDecoration(uint id, uint memberIndex, SpvDecoration decoration)
        {
            return spvc_compiler_get_member_decoration(Handle, id, memberIndex, decoration);
        }

        public string GetMemberDecorationString(uint id, uint memberIndex, SpvDecoration decoration)
        {
            return Marshal.PtrToStringAnsi(spvc_compiler_get_member_decoration_string(Handle, id, memberIndex, decoration));
        }

        public string GetMemberName(uint id, uint memberIndex)
        {
            return Marshal.PtrToStringAnsi(spvc_compiler_get_member_name(Handle, id, memberIndex));
        }

        public uint BuildDummySamplerForCombinedImages()
        {
            spvc_compiler_build_dummy_sampler_for_combined_images(Handle, out var id).CheckError();
            return id;
        }

        public void BuildCombinedImageSamplers()
        {
            spvc_compiler_build_combined_image_samplers(Handle).CheckError();
        }

        public CombinedImageSampler[] GetCombinedImageSamplers()
        {
            unsafe
            {
                CombinedImageSampler* samplers_ptr;
                spvc_compiler_get_combined_image_samplers(Handle,
                    &samplers_ptr,
                    out var num_samplers).CheckError();

                var samplers = new CombinedImageSampler[num_samplers.ToInt32()];

                for (int i = 0; i < samplers.Length; i++)
                {
                    samplers[i] = new CombinedImageSampler
                    {
                        CombinedId = samplers_ptr[i].CombinedId,
                        ImageId = samplers_ptr[i].ImageId,
                        SamplerId = samplers_ptr[i].SamplerId
                    };
                }

                return samplers;
            }
        }

        public SpirvType GetSpirvType(uint id)
        {
            if (id == 0)
                return null;

            var handle = spvc_compiler_get_type_handle(Handle, id);
            if (handle == IntPtr.Zero)
                return null;

            return new SpirvType(this, handle);
        }

        public bool GetBinaryOffsetForDecoration(uint id, SpvDecoration decoration, out uint wordOffset)
        {
            return spvc_compiler_get_binary_offset_for_decoration(Handle, id, decoration, out wordOffset);
        }

        public bool BufferIsHlslCounterBuffer(uint id)
        {
            return spvc_compiler_buffer_is_hlsl_counter_buffer(Handle, id);
        }

        public bool BufferGetHlslCounterBuffer(uint id, out uint counterId)
        {
            return spvc_compiler_buffer_get_hlsl_counter_buffer(Handle, id, out counterId);
        }

        public SpvCapability[] GetDeclaredCapabilities()
        {
            unsafe
            {
                SpvCapability* capabilities_ptr;
                spvc_compiler_get_declared_capabilities(Handle,
                    &capabilities_ptr,
                    out var num_capabilities).CheckError();

                var capabilities = new SpvCapability[num_capabilities.ToInt32()];

                for (int i = 0; i < capabilities.Length; i++)
                {
                    capabilities[i] = capabilities_ptr[i];
                }

                return capabilities;
            }
        }

        public string[] GetDeclaredExtensions()
        {
            unsafe
            {
                IntPtr* extensions_ptr;
                spvc_compiler_get_declared_extensions(Handle,
                    &extensions_ptr,
                    out var num_extensions).CheckError();

                var extensions = new string[num_extensions.ToInt32()];

                for (int i = 0; i < extensions.Length; i++)
                {
                    extensions[i] = Marshal.PtrToStringAnsi(extensions_ptr[i]);
                }

                return extensions;
            }
        }

        public string GetRemappedDeclaredBlockName(uint id)
        {
            return Marshal.PtrToStringAnsi(spvc_compiler_get_remapped_declared_block_name(Handle, id));
        }

        public SpvDecoration[] GetBufferBlockDecorations(uint id)
        {
            unsafe
            {
                SpvDecoration* decorations_ptr;
                spvc_compiler_get_buffer_block_decorations(Handle,
                    id,
                    &decorations_ptr,
                    out var num_decorations).CheckError();

                var decorations = new SpvDecoration[num_decorations.ToInt32()];

                for (int i = 0; i < decorations.Length; i++)
                {
                    decorations[i] = decorations_ptr[i];
                }

                return decorations;
            }
        }

        public bool GetDeclaredStructSize(SpirvType structType, out int size)
        {
            if (spvc_compiler_get_declared_struct_size(Handle, structType.Handle, out IntPtr size_ptr) != Result.Success)
            {
                size = 0;
                return false;
            }

            size = size_ptr.ToInt32();
            return true;
        }

        public bool GetDeclaredStructMemberSize(SpirvType structType, int index, out int size)
        {
            if (spvc_compiler_get_declared_struct_member_size(Handle, structType.Handle, (uint)index, out IntPtr size_ptr) != Result.Success)
            {
                size = 0;
                return false;
            }

            size = size_ptr.ToInt32();
            return true;
        }

        public bool GetStructMemberOffset(SpirvType type, int index, out int offset)
        {
            return spvc_compiler_type_struct_member_offset(Handle, type.Handle, index, out offset) == Result.Success;
        }

        public bool GetStructMemberArrayStride(SpirvType type, int index, out int stride)
        {
            return spvc_compiler_type_struct_member_array_stride(Handle, type.Handle, index, out stride) == Result.Success;
        }

        public bool GetStructMemberMatrixStride(SpirvType type, int index, out int stride)
        {
            return spvc_compiler_type_struct_member_matrix_stride(Handle, type.Handle, index, out stride) == Result.Success;
        }

 
    }
}
