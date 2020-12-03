



using System;
using System.Runtime.InteropServices;

namespace Shaderc
{
    public static unsafe class Native
    {
        internal delegate IntPtr PFN_shaderc_compiler_initialize();
        internal static PFN_shaderc_compiler_initialize shaderc_compiler_initialize_ = Loader.GetStaticProc<PFN_shaderc_compiler_initialize>("shaderc_compiler_initialize");
        public static IntPtr shaderc_compiler_initialize()
        {
            return shaderc_compiler_initialize_();
        }

        internal delegate void PFN_shaderc_compiler_release(IntPtr compiler);
        internal static readonly PFN_shaderc_compiler_release shaderc_compiler_release_ = Loader.GetStaticProc<PFN_shaderc_compiler_release>("shaderc_compiler_release");
        public static void shaderc_compiler_release(IntPtr compiler)
        {
            shaderc_compiler_release_(compiler);
        }

        internal delegate IntPtr PFN_shaderc_compile_options_initialize();
        internal static readonly PFN_shaderc_compile_options_initialize shaderc_compile_options_initialize_ = Loader.GetStaticProc<PFN_shaderc_compile_options_initialize>("shaderc_compile_options_initialize");
        public static IntPtr shaderc_compile_options_initialize()
        {
            return shaderc_compile_options_initialize_();
        }

        internal delegate void PFN_shaderc_compile_options_release(IntPtr options);
        internal static readonly PFN_shaderc_compile_options_release shaderc_compile_options_release_ = Loader.GetStaticProc<PFN_shaderc_compile_options_release>("shaderc_compile_options_release");
        public static void shaderc_compile_options_release(IntPtr options)
        {
            shaderc_compile_options_release_(options);
        }

        internal delegate void PFN_shaderc_result_release(IntPtr options);
        internal static readonly PFN_shaderc_result_release shaderc_result_release_ = Loader.GetStaticProc<PFN_shaderc_result_release>("shaderc_result_release");
        public static void shaderc_result_release(IntPtr options)
        {
            shaderc_result_release_(options);
        }

        internal delegate ulong PFN_shaderc_result_get_length(IntPtr result);
        internal static readonly PFN_shaderc_result_get_length shaderc_result_get_length_ = Loader.GetStaticProc<PFN_shaderc_result_get_length>("shaderc_result_get_length");
        public static ulong shaderc_result_get_length(IntPtr result)
        {
            return shaderc_result_get_length_(result);
        }

        internal delegate ulong PFN_shaderc_result_get_num_warnings(IntPtr result);
        internal static readonly PFN_shaderc_result_get_num_warnings shaderc_result_get_num_warnings_ = Loader.GetStaticProc<PFN_shaderc_result_get_num_warnings>("shaderc_result_get_num_warnings");
        public static ulong shaderc_result_get_num_warnings(IntPtr result)
        {
            return shaderc_result_get_num_warnings_(result);
        }

        internal delegate ulong PFN_shaderc_result_get_num_errors(IntPtr result);
        internal static readonly PFN_shaderc_result_get_num_errors shaderc_result_get_num_errors_ = Loader.GetStaticProc<PFN_shaderc_result_get_num_errors>("shaderc_result_get_num_errors");
        public static ulong shaderc_result_get_num_errors(IntPtr result)
        {
            return shaderc_result_get_num_errors_(result);
        }

        internal delegate Status PFN_shaderc_result_get_compilation_status(IntPtr result);
        internal static readonly PFN_shaderc_result_get_compilation_status shaderc_result_get_compilation_status_ = Loader.GetStaticProc<PFN_shaderc_result_get_compilation_status>("shaderc_result_get_compilation_status");
        public static Status shaderc_result_get_compilation_status(IntPtr result)
        {
            return shaderc_result_get_compilation_status_(result);
        }

        internal delegate byte* PFN_shaderc_result_get_bytes(IntPtr result);
        internal static readonly PFN_shaderc_result_get_bytes shaderc_result_get_bytes_ = Loader.GetStaticProc<PFN_shaderc_result_get_bytes>("shaderc_result_get_bytes");
        public static byte* shaderc_result_get_bytes(IntPtr result)
        {
            return shaderc_result_get_bytes_(result);
        }

        internal delegate IntPtr PFN_shaderc_result_get_error_message(IntPtr result);
        internal static readonly PFN_shaderc_result_get_error_message shaderc_result_get_error_message_ = Loader.GetStaticProc<PFN_shaderc_result_get_error_message>("shaderc_result_get_error_message");
        public static IntPtr shaderc_result_get_error_message(IntPtr result)
        {
            return shaderc_result_get_error_message_(result);
        }

        internal delegate IntPtr PFN_shaderc_get_spv_version(SpirVVersion* version, uint* revision);
        internal static readonly PFN_shaderc_get_spv_version shaderc_get_spv_version_ = Loader.GetStaticProc<PFN_shaderc_get_spv_version>("shaderc_get_spv_version");
        public static IntPtr shaderc_get_spv_version(SpirVVersion* version, uint* revision)
        {
            return shaderc_get_spv_version_(version, revision);
        }

        internal delegate IntPtr PFN_shaderc_parse_version_profile(string str, int* version, Profile* profile);
        internal static readonly PFN_shaderc_parse_version_profile shaderc_parse_version_profile_ = Loader.GetStaticProc<PFN_shaderc_parse_version_profile>("shaderc_parse_version_profile");
        public static IntPtr shaderc_parse_version_profile(string str, int* version, Profile* profile)
        {
            return shaderc_parse_version_profile_(str, version, profile);
        }

        internal delegate IntPtr PFN_shaderc_compile_options_set_include_callbacks(IntPtr options, IntPtr resolver, IntPtr result_releaser, IntPtr user_data);
        internal static readonly PFN_shaderc_compile_options_set_include_callbacks shaderc_compile_options_set_include_callbacks_ = Loader.GetStaticProc<PFN_shaderc_compile_options_set_include_callbacks>("shaderc_compile_options_set_include_callbacks");
        public static IntPtr shaderc_compile_options_set_include_callbacks(IntPtr options, IntPtr resolver, IntPtr result_releaser, IntPtr user_data)
        {
            return shaderc_compile_options_set_include_callbacks_(options, resolver, result_releaser, user_data);
        }

        internal delegate IntPtr PFN_shaderc_compile_options_clone(IntPtr options);
        internal static readonly PFN_shaderc_compile_options_clone shaderc_compile_options_clone_ = Loader.GetStaticProc<PFN_shaderc_compile_options_clone>("shaderc_compile_options_clone");
        public static IntPtr shaderc_compile_options_clone(IntPtr options)
        {
            return shaderc_compile_options_clone_(options);
        }

        internal delegate IntPtr PFN_shaderc_compile_options_add_macro_definition(IntPtr options, string name, ulong name_length, string value, ulong value_length);
        internal static readonly PFN_shaderc_compile_options_add_macro_definition shaderc_compile_options_add_macro_definition_ = Loader.GetStaticProc<PFN_shaderc_compile_options_add_macro_definition>("shaderc_compile_options_add_macro_definition");
        public static IntPtr shaderc_compile_options_add_macro_definition(IntPtr options, string name, ulong name_length, string value, ulong value_length)
        {
            return shaderc_compile_options_add_macro_definition_(options, name, name_length, value, value_length);
        }

        internal delegate IntPtr PFN_shaderc_compile_options_set_source_language(IntPtr options, SourceLanguage lang);
        internal static readonly PFN_shaderc_compile_options_set_source_language shaderc_compile_options_set_source_language_ = Loader.GetStaticProc<PFN_shaderc_compile_options_set_source_language>("shaderc_compile_options_set_source_language");
        public static IntPtr shaderc_compile_options_set_source_language(IntPtr options, SourceLanguage lang)
        {
            return shaderc_compile_options_set_source_language_(options, lang);
        }

        internal delegate IntPtr PFN_shaderc_compile_options_set_generate_debug_info(IntPtr options);
        internal static readonly PFN_shaderc_compile_options_set_generate_debug_info shaderc_compile_options_set_generate_debug_info_ = Loader.GetStaticProc<PFN_shaderc_compile_options_set_generate_debug_info>("shaderc_compile_options_set_generate_debug_info");
        public static IntPtr shaderc_compile_options_set_generate_debug_info(IntPtr options)
        {
            return shaderc_compile_options_set_generate_debug_info_(options);
        }

        internal delegate IntPtr PFN_shaderc_compile_options_set_optimization_level(IntPtr options, OptimizationLevel level);
        internal static readonly PFN_shaderc_compile_options_set_optimization_level shaderc_compile_options_set_optimization_level_ = Loader.GetStaticProc<PFN_shaderc_compile_options_set_optimization_level>("shaderc_compile_options_set_optimization_level");
        public static IntPtr shaderc_compile_options_set_optimization_level(IntPtr options, OptimizationLevel level)
        {
            return shaderc_compile_options_set_optimization_level_(options, level);
        }

        internal delegate IntPtr PFN_shaderc_compile_options_set_forced_version_profile(IntPtr options, int version, Profile profile);
        internal static readonly PFN_shaderc_compile_options_set_forced_version_profile shaderc_compile_options_set_forced_version_profile_ = Loader.GetStaticProc<PFN_shaderc_compile_options_set_forced_version_profile>("shaderc_compile_options_set_forced_version_profile");
        public static IntPtr shaderc_compile_options_set_forced_version_profile(IntPtr options, int version, Profile profile)
        {
            return shaderc_compile_options_set_forced_version_profile_(options, version, profile);
        }

        internal delegate IntPtr PFN_shaderc_compile_options_set_suppress_warnings(IntPtr options);
        internal static readonly PFN_shaderc_compile_options_set_suppress_warnings shaderc_compile_options_set_suppress_warnings_ = Loader.GetStaticProc<PFN_shaderc_compile_options_set_suppress_warnings>("shaderc_compile_options_set_suppress_warnings");
        public static IntPtr shaderc_compile_options_set_suppress_warnings(IntPtr options)
        {
            return shaderc_compile_options_set_suppress_warnings_(options);
        }

        internal delegate IntPtr PFN_shaderc_compile_options_set_target_env(IntPtr options, TargetEnvironment target, EnvironmentVersion version);
        internal static readonly PFN_shaderc_compile_options_set_target_env shaderc_compile_options_set_target_env_ = Loader.GetStaticProc<PFN_shaderc_compile_options_set_target_env>("shaderc_compile_options_set_target_env");
        public static IntPtr shaderc_compile_options_set_target_env(IntPtr options, TargetEnvironment target, EnvironmentVersion version)
        {
            return shaderc_compile_options_set_target_env_(options, target, version);
        }

        internal delegate IntPtr PFN_shaderc_compile_options_set_target_spirv(IntPtr options, SpirVVersion version);
        internal static readonly PFN_shaderc_compile_options_set_target_spirv shaderc_compile_options_set_target_spirv_ = Loader.GetStaticProc<PFN_shaderc_compile_options_set_target_spirv>("shaderc_compile_options_set_target_spirv");
        public static IntPtr shaderc_compile_options_set_target_spirv(IntPtr options, SpirVVersion version)
        {
            return shaderc_compile_options_set_target_spirv_(options, version);
        }

        internal delegate IntPtr PFN_shaderc_compile_options_set_warnings_as_errors(IntPtr option);
        internal static readonly PFN_shaderc_compile_options_set_warnings_as_errors shaderc_compile_options_set_warnings_as_errors_ = Loader.GetStaticProc<PFN_shaderc_compile_options_set_warnings_as_errors>("shaderc_compile_options_set_warnings_as_errors");
        public static IntPtr shaderc_compile_options_set_warnings_as_errors(IntPtr options)
        {
            return shaderc_compile_options_set_warnings_as_errors_(options);
        }

        internal delegate IntPtr PFN_shaderc_compile_options_set_limit(IntPtr options, Limit limit, int value);
        internal static readonly PFN_shaderc_compile_options_set_limit shaderc_compile_options_set_limit_ = Loader.GetStaticProc<PFN_shaderc_compile_options_set_limit>("shaderc_compile_options_set_limit");
        public static IntPtr shaderc_compile_options_set_limit(IntPtr options, Limit limit, int value)
        {
            return shaderc_compile_options_set_limit_(options, limit, value);
        }

        internal delegate IntPtr PFN_shaderc_compile_options_set_auto_bind_uniforms(IntPtr options, bool auto_bind);
        internal static readonly PFN_shaderc_compile_options_set_auto_bind_uniforms shaderc_compile_options_set_auto_bind_uniforms_ = Loader.GetStaticProc<PFN_shaderc_compile_options_set_auto_bind_uniforms>("shaderc_compile_options_set_auto_bind_uniforms");
        public static IntPtr shaderc_compile_options_set_auto_bind_uniforms(IntPtr options, bool auto_bind)
        {
            return shaderc_compile_options_set_auto_bind_uniforms_(options, auto_bind);
        }

        internal delegate IntPtr PFN_shaderc_compile_options_set_hlsl_io_mapping(IntPtr options, bool hlsl_iomap);
        internal static readonly PFN_shaderc_compile_options_set_hlsl_io_mapping shaderc_compile_options_set_hlsl_io_mapping_ = Loader.GetStaticProc<PFN_shaderc_compile_options_set_hlsl_io_mapping>("shaderc_compile_options_set_hlsl_io_mapping");
        public static IntPtr shaderc_compile_options_set_hlsl_io_mapping(IntPtr options, bool hlsl_iomap)
        {
            return shaderc_compile_options_set_hlsl_io_mapping_(options, hlsl_iomap);
        }

        internal delegate IntPtr PFN_shaderc_compile_options_set_hlsl_offsets(IntPtr options, bool hlsl_offsets);
        internal static readonly PFN_shaderc_compile_options_set_hlsl_offsets shaderc_compile_options_set_hlsl_offsets_ = Loader.GetStaticProc<PFN_shaderc_compile_options_set_hlsl_offsets>("shaderc_compile_options_set_hlsl_offsets");
        public static IntPtr shaderc_compile_options_set_hlsl_offsets(IntPtr options, bool hlsl_offsets)
        {
            return shaderc_compile_options_set_hlsl_offsets_(options, hlsl_offsets);
        }

        internal delegate IntPtr PFN_shaderc_compile_options_set_binding_base(IntPtr options, UniformKind kind, uint _base);
        internal static readonly PFN_shaderc_compile_options_set_binding_base shaderc_compile_options_set_binding_base_ = Loader.GetStaticProc<PFN_shaderc_compile_options_set_binding_base>("shaderc_compile_options_set_binding_base");
        public static IntPtr shaderc_compile_options_set_binding_base(IntPtr options, UniformKind kind, uint _base)
        {
            return shaderc_compile_options_set_binding_base_(options, kind, _base);
        }

        internal delegate IntPtr PFN_shaderc_compile_options_set_binding_base_for_stage(IntPtr options, ShaderKind shader_kind, UniformKind kind, uint _base);
        internal static readonly PFN_shaderc_compile_options_set_binding_base_for_stage shaderc_compile_options_set_binding_base_for_stage_ = Loader.GetStaticProc<PFN_shaderc_compile_options_set_binding_base_for_stage>("shaderc_compile_options_set_binding_base_for_stage");
        public static IntPtr shaderc_compile_options_set_binding_base_for_stage(IntPtr options, ShaderKind shader_kind, UniformKind kind, uint _base)
        {
            return shaderc_compile_options_set_binding_base_for_stage_(options, shader_kind, kind, _base);
        }

        internal delegate IntPtr PFN_shaderc_compile_options_set_auto_map_locations(IntPtr options, bool auto_map);
        internal static readonly PFN_shaderc_compile_options_set_auto_map_locations shaderc_compile_options_set_auto_map_locations_ = Loader.GetStaticProc<PFN_shaderc_compile_options_set_auto_map_locations>("shaderc_compile_options_set_auto_map_locations");
        public static IntPtr shaderc_compile_options_set_auto_map_locations(IntPtr options, bool auto_map)
        {
            return shaderc_compile_options_set_auto_map_locations_(options, auto_map);
        }

        internal delegate IntPtr PFN_shaderc_compile_options_set_hlsl_register_set_and_binding_for_stage(IntPtr options, ShaderKind shader_kind, string reg, string set, string binding);
        internal static readonly PFN_shaderc_compile_options_set_hlsl_register_set_and_binding_for_stage shaderc_compile_options_set_hlsl_register_set_and_binding_for_stage_ = Loader.GetStaticProc<PFN_shaderc_compile_options_set_hlsl_register_set_and_binding_for_stage>("shaderc_compile_options_set_hlsl_register_set_and_binding_for_stage");
        public static IntPtr shaderc_compile_options_set_hlsl_register_set_and_binding_for_stage(IntPtr options, ShaderKind shader_kind, string reg, string set, string binding)
        {
            return shaderc_compile_options_set_hlsl_register_set_and_binding_for_stage_(options, shader_kind, reg, set, binding);
        }

        internal delegate IntPtr PFN_shaderc_compile_options_set_hlsl_register_set_and_binding(IntPtr options, string reg, string set, string binding);
        internal static readonly PFN_shaderc_compile_options_set_hlsl_register_set_and_binding shaderc_compile_options_set_hlsl_register_set_and_binding_ = Loader.GetStaticProc<PFN_shaderc_compile_options_set_hlsl_register_set_and_binding>("shaderc_compile_options_set_hlsl_register_set_and_binding");
        public static IntPtr shaderc_compile_options_set_hlsl_register_set_and_binding(IntPtr options, string reg, string set, string binding)
        {
            return shaderc_compile_options_set_hlsl_register_set_and_binding_(options, reg, set, binding);
        }

        internal delegate IntPtr PFN_shaderc_compile_options_set_hlsl_functionality1(IntPtr options, bool enable);
        internal static readonly PFN_shaderc_compile_options_set_hlsl_functionality1 shaderc_compile_options_set_hlsl_functionality1_ = Loader.GetStaticProc<PFN_shaderc_compile_options_set_hlsl_functionality1>("shaderc_compile_options_set_hlsl_functionality1");
        public static IntPtr shaderc_compile_options_set_hlsl_functionality1(IntPtr options, bool enable)
        {
            return shaderc_compile_options_set_hlsl_functionality1_(options, enable);
        }

        internal delegate IntPtr PFN_shaderc_compile_options_set_invert_y(IntPtr options, bool enable);
        internal static readonly PFN_shaderc_compile_options_set_invert_y shaderc_compile_options_set_invert_y_ = Loader.GetStaticProc<PFN_shaderc_compile_options_set_invert_y>("shaderc_compile_options_set_invert_y");
        public static IntPtr shaderc_compile_options_set_invert_y(IntPtr options, bool enable)
        {
            return shaderc_compile_options_set_invert_y_(options, enable);
        }

        internal delegate IntPtr PFN_shaderc_compile_options_set_nan_clamp(IntPtr options, bool enable);
        internal static readonly PFN_shaderc_compile_options_set_nan_clamp shaderc_compile_options_set_nan_clamp_ = Loader.GetStaticProc<PFN_shaderc_compile_options_set_nan_clamp>("shaderc_compile_options_set_nan_clamp");
        public static IntPtr shaderc_compile_options_set_nan_clamp(IntPtr options, bool enable)
        {
            return shaderc_compile_options_set_nan_clamp_(options, enable);
        }

        internal delegate IntPtr PFN_shaderc_compile_into_spv(IntPtr compiler, byte* source, ulong source_size, byte shader_kind, byte* input_file, byte* entry_point, IntPtr additional_options);
        internal static readonly PFN_shaderc_compile_into_spv shaderc_compile_into_spv_ = Loader.GetStaticProc<PFN_shaderc_compile_into_spv>("shaderc_compile_into_spv");
        public static IntPtr shaderc_compile_into_spv(IntPtr compiler, byte* source, ulong source_size, byte shader_kind, byte* input_file, byte* entry_point, IntPtr additional_options)
        {
            return shaderc_compile_into_spv_(compiler, source, source_size, shader_kind, input_file, entry_point, additional_options);
        }
    }
}
