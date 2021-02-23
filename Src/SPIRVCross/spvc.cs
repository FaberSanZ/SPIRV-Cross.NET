// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using spvc_context = System.IntPtr;
using spvc_compiler = System.IntPtr;
using spvc_compiler_options = System.IntPtr;
using spvc_variable_id = System.UInt32;
using spvc_resources = System.IntPtr;
using spvc_type_id = System.UInt32;
using spvc_set = System.IntPtr;
using System.IO;

namespace SharpSPIRVCross
{
    internal static unsafe class spvc
    {
        private static readonly ILibraryLoader s_loader = InitializeLoader();
        private static readonly IntPtr s_NativeLibrary = LoadNativeLibrary();



        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate Result PFN_spvc_context_createFunc(out spvc_context context);
        private static PFN_spvc_context_createFunc spvc_context_create_ = LoadFunction<PFN_spvc_context_createFunc>(nameof(spvc_context_create));
        public static Result spvc_context_create(out spvc_context context) => spvc_context_create_(out context);









        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static extern void spvc_context_destroy(spvc_context context);



        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static extern void spvc_context_release_allocations(spvc_context context);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void spvc_error_callback(IntPtr userData, [MarshalAs(UnmanagedType.LPStr)] string description);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static extern void spvc_context_set_error_callback(spvc_context context, spvc_error_callback callback, IntPtr userData);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr spvc_context_get_last_error_string(spvc_context context);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result spvc_context_parse_spirv(spvc_context context, void* spirv, IntPtr word_count, out IntPtr parsed_ir);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result spvc_context_create_compiler(spvc_context context, Backend backend,
                                                        IntPtr parsed_ir, CaptureMode mode,
                                                        out spvc_compiler compiler);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result spvc_compiler_create_compiler_options(spvc_compiler compiler, out spvc_compiler_options options);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result spvc_compiler_options_set_bool(spvc_compiler_options options, CompilerOption option, [MarshalAs(UnmanagedType.I1)] bool value);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result spvc_compiler_options_set_uint(spvc_compiler_options options, CompilerOption option, uint value);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result spvc_compiler_install_compiler_options(spvc_compiler compiler, spvc_compiler_options options);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result spvc_compiler_compile(spvc_compiler compiler, out IntPtr source);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result spvc_compiler_add_header_line(spvc_compiler compiler, string line);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result spvc_compiler_require_extension(spvc_compiler compiler, string extension);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result spvc_compiler_flatten_buffer_block(spvc_compiler compiler, spvc_variable_id id);

        #region HLSL Options
        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result spvc_compiler_hlsl_set_root_constants_layout(spvc_compiler compiler, HLSLRootConstants* constant_info, IntPtr count);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result spvc_compiler_hlsl_add_vertex_attribute_remap(spvc_compiler compiler, HLSLVertexAttributeRemap.__Native* remap, IntPtr remaps);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static extern spvc_variable_id spvc_compiler_hlsl_remap_num_workgroups_builtin(spvc_compiler compiler);
        #endregion

        #region MSL Options
        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool spvc_compiler_msl_is_rasterization_disabled(spvc_compiler compiler);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool spvc_compiler_msl_needs_aux_buffer(spvc_compiler compiler);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool spvc_compiler_msl_needs_output_buffer(spvc_compiler compiler);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool spvc_compiler_msl_needs_patch_output_buffer(spvc_compiler compiler);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool spvc_compiler_msl_needs_input_threadgroup_mem(spvc_compiler compiler);
        #endregion

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result spvc_compiler_get_active_interface_variables(spvc_compiler compiler, out spvc_set set);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result spvc_compiler_set_enabled_interface_variables(spvc_compiler compiler, spvc_set set);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result spvc_compiler_create_shader_resources(spvc_compiler compiler, out spvc_resources resources);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result spvc_compiler_create_shader_resources_for_active_variables(spvc_compiler compiler, out spvc_resources resources, spvc_set active);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result spvc_resources_get_resource_list_for_type(
            spvc_resources resources,
            ResourceType resourceType,
            ReflectedResource.__Native** resource_list,
            out IntPtr resource_size);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static extern void spvc_compiler_set_decoration(spvc_compiler compiler, uint id, SpvDecoration decoration, uint argument);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static extern void spvc_compiler_set_decoration_string(spvc_compiler compiler, uint id, SpvDecoration decoration, string argument);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static extern void spvc_compiler_set_name(spvc_compiler compiler, uint id, string argument);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static extern void spvc_compiler_set_member_decoration(spvc_compiler compiler, uint id, uint member_index,
                                                         SpvDecoration decoration, uint argument);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static extern void spvc_compiler_set_member_decoration_string(spvc_compiler compiler, uint id, uint member_index,
                                                         SpvDecoration decoration, string argument);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static extern void spvc_compiler_set_member_name(spvc_compiler compiler, uint id, uint member_index, string argument);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static extern void spvc_compiler_unset_decoration(spvc_compiler compiler, uint id, SpvDecoration decoration);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static extern void spvc_compiler_unset_member_decoration(spvc_compiler compiler, uint id,
                                                           uint member_index, SpvDecoration decoration);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool spvc_compiler_has_decoration(spvc_compiler compiler, uint id, SpvDecoration decoration);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool spvc_compiler_has_member_decoration(spvc_compiler compiler, uint id,
                                                              uint member_index, SpvDecoration decoration);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr spvc_compiler_get_name(spvc_compiler compiler, uint id);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint spvc_compiler_get_decoration(spvc_compiler compiler, uint id, SpvDecoration decoration);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr spvc_compiler_get_decoration_string(spvc_compiler compiler, uint id, SpvDecoration decoration);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint spvc_compiler_get_member_decoration(spvc_compiler compiler, uint id, uint memberIndex, SpvDecoration decoration);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr spvc_compiler_get_member_decoration_string(spvc_compiler compiler, uint id, uint memberIndex, SpvDecoration decoration);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr spvc_compiler_get_member_name(spvc_compiler compiler, uint id, uint memberIndex);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result spvc_compiler_build_dummy_sampler_for_combined_images(spvc_compiler compiler, out spvc_variable_id id);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result spvc_compiler_build_combined_image_samplers(spvc_compiler compiler);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result spvc_compiler_get_combined_image_samplers(spvc_compiler compiler, 
            CombinedImageSampler** samplers,
            out IntPtr num_samplers);


        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr spvc_compiler_get_type_handle(IntPtr compiler, uint id);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static extern SpvExecutionModel spvc_compiler_get_execution_model(IntPtr compiler);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool spvc_compiler_get_binary_offset_for_decoration(IntPtr compiler, uint id, SpvDecoration decoration, out uint word_offset);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool spvc_compiler_buffer_is_hlsl_counter_buffer(IntPtr compiler, uint id);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool spvc_compiler_buffer_get_hlsl_counter_buffer(IntPtr compiler, uint id, out uint counter_id);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern Result spvc_compiler_get_declared_capabilities(IntPtr compiler, SpvCapability** capabilities, out IntPtr num_capabilities);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern Result spvc_compiler_get_declared_extensions(IntPtr compiler, IntPtr** extensions, out IntPtr num_extensions);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern IntPtr spvc_compiler_get_remapped_declared_block_name(IntPtr compiler, uint id);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern Result spvc_compiler_get_buffer_block_decorations(IntPtr compiler, uint id, SpvDecoration** decorations, out IntPtr num_decorations);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern Result spvc_compiler_get_declared_struct_size(IntPtr compiler, IntPtr struct_type, out IntPtr size);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern Result spvc_compiler_get_declared_struct_member_size(IntPtr compiler, IntPtr struct_type, uint index, out IntPtr size);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern Result spvc_compiler_type_struct_member_offset(IntPtr compiler, IntPtr type, int index, out int offset);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern Result spvc_compiler_type_struct_member_array_stride(IntPtr compiler, IntPtr type, int index, out int stride);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern Result spvc_compiler_type_struct_member_matrix_stride(IntPtr compiler, IntPtr type, int index, out int stride);


        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern Result spvc_compiler_get_active_buffer_ranges(IntPtr compiler, IntPtr type, int index, out int stride);



        #region NativeLibrary Logic
        private static IntPtr LoadNativeLibrary()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return LoadLibrary("cspirv_cross.dll");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return LoadLibrary("libcspirv_cross.so");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return LoadLibrary("libcspirv_cross.dylib");
            }
            else
            {
                return LoadLibrary("cspirv_cross");
            }
        }

        private static IntPtr LoadLibrary(string libname)
        {
            string? assemblyLocation = Path.GetDirectoryName(typeof(spvc).Assembly.Location) ?? "./";
            IntPtr ret;

            // Try .NET Framework / mono locations
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                ret = s_loader.LoadLibrary(Path.Combine(assemblyLocation, libname));

                // Look in Frameworks for .app bundles
                if (ret == IntPtr.Zero)
                    ret = s_loader.LoadLibrary(Path.Combine(assemblyLocation, "..", "Frameworks", libname));
            }
            else
            {
                if (Environment.Is64BitProcess)
                    ret = s_loader.LoadLibrary(Path.Combine(assemblyLocation, "x64", libname));
                else
                    ret = s_loader.LoadLibrary(Path.Combine(assemblyLocation, "x86", libname));
            }

            // Try .NET Core development locations
            if (ret == IntPtr.Zero)
                ret = s_loader.LoadLibrary(Path.Combine(assemblyLocation, "native", Rid, libname));

            if (ret == IntPtr.Zero)
                ret = s_loader.LoadLibrary(Path.Combine(assemblyLocation, "runtimes", Rid, "native", libname));

            // Try current folder (.NET Core will copy it there after publish) or system library
            if (ret == IntPtr.Zero)
                ret = s_loader.LoadLibrary(libname);

            // Welp, all failed, PANIC!!!
            if (ret == IntPtr.Zero)
                throw new Exception("Failed to load library: " + libname);

            return ret;
        }

        private static T LoadFunction<T>(string function)
        {
            IntPtr handle = s_loader.GetSymbol(s_NativeLibrary, function);

            if (handle == IntPtr.Zero)
            {
                throw new EntryPointNotFoundException(function);
            }

            return Marshal.GetDelegateForFunctionPointer<T>(handle);
        }

        private static ILibraryLoader InitializeLoader()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return new WindowsLoader();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return new OSXLoader();
            }
            else
            {
                return new UnixLoader();
            }
        }

        private static string Rid
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && Environment.Is64BitProcess)
                    return "win-x64";
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !Environment.Is64BitProcess)
                    return "win-x86";
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    return "linux-x64";
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    return "osx";
                else
                    return "unknown";
            }
        }


        internal interface ILibraryLoader
        {
            IntPtr LoadLibrary(string name);

            IntPtr GetSymbol(IntPtr module, string name);
        }

        private class WindowsLoader : ILibraryLoader
        {
            public IntPtr LoadLibrary(string name) => LoadLibraryW(name);

            public IntPtr GetSymbol(IntPtr module, string name) => GetProcAddress(module, name);

            [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
            private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

            [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
            private static extern IntPtr LoadLibraryW(string lpszLib);

            [DllImport("kernel32")]
            private static extern int FreeLibrary(IntPtr module);
        }

        private class UnixLoader : ILibraryLoader
        {
            private const int RTLD_LOCAL = 0x0000;
            private const int RTLD_NOW = 0x0002;

            public IntPtr LoadLibrary(string name) => dlopen(name, RTLD_NOW | RTLD_LOCAL);

            public IntPtr GetSymbol(IntPtr module, string name) => dlsym(module, name);


            [DllImport("libdl.so.2")]
            private static extern IntPtr dlopen(string path, int flags);

            [DllImport("libdl.so.2")]
            private static extern IntPtr dlsym(IntPtr handle, string symbol);
        }

        private class OSXLoader : ILibraryLoader
        {
            private const int RTLD_LOCAL = 0x0000;
            private const int RTLD_NOW = 0x0002;

            public IntPtr LoadLibrary(string name) => dlopen(name, RTLD_NOW | RTLD_LOCAL);

            public IntPtr GetSymbol(IntPtr module, string name) => dlsym(module, name);


            [DllImport("/usr/lib/libSystem.dylib")]
            private static extern IntPtr dlopen(string path, int flags);

            [DllImport("/usr/lib/libSystem.dylib")]
            private static extern IntPtr dlsym(IntPtr handle, string symbol);
        }
        #endregion  NativeLibrary Logic


    }
}
