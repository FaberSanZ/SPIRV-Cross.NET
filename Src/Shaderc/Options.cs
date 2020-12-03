// Copyright (c) 2020 - 2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*===================================================================================
	Options.cs
====================================================================================*/


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Shaderc
{

    public class Options : IDisposable, ICloneable
    {
        internal IntPtr handle;
        internal static int curId;
        internal readonly int id;
        internal readonly bool includeEnabled;

        internal static Dictionary<int, Options> optionsDic = new Dictionary<int, Options>();//context data for callbacks


        internal static PFN_IncludeResolve resolve = HandlePFN_IncludeResolve;
        internal static PFN_IncludeResultRelease incResultRelease = HandlePFN_IncludeResultRelease;




        /// <summary>
        /// Create a new instance of the Options class.
        /// </summary>
        /// <param name="enableIncludes">If set to 'true' include resolution is activated</param>
        public Options(bool enableIncludes = true) : this(Native.shaderc_compile_options_initialize(), enableIncludes) { }


        public Options(IntPtr handle, bool enableIncludes)
        {
            this.handle = handle;

            if (handle == IntPtr.Zero)
                throw new Exception("error");

            id = curId++;

            optionsDic.Add(id, this);

            includeEnabled = enableIncludes;

            if (enableIncludes) setIncludeCallbacks();
        }




        /// <summary>
        /// List of the pathes to search when trying to resolve a 'Standard' include (enclosed in $lt;>)
        /// May be absolute pathes or relative to the executable directory.
        /// </summary>
        public readonly List<string> IncludeDirectories = new List<string>();


        /// <summary>
        /// Sets the source language.  The default is GLSL.
        /// </summary>
        public SourceLanguage SourceLanguage 
        { 
            set => Native.shaderc_compile_options_set_source_language(handle, value); 
        }

        /// <summary>
        /// Sets the compiler optimization level to the given level.
        /// </summary>
        public OptimizationLevel Optimization 
        { 
            set => Native.shaderc_compile_options_set_optimization_level(handle, value); 
        }



        /// <summary>
        /// Sets whether the compiler should automatically assign bindings to uniforms
        /// that aren't already explicitly bound in the shader source.
        /// </summary>
        public bool AutoBindUniforms
        {
            set => Native.shaderc_compile_options_set_auto_bind_uniforms(handle, value);
        }


        /// <summary>
        /// Sets whether the compiler should use HLSL IO mapping rules for bindings.
        /// Defaults to false.
        /// </summary>
        public bool HlslIoMapping
        {
            set => Native.shaderc_compile_options_set_hlsl_io_mapping(handle, value);
        }


        /// <summary>
        /// Sets whether the compiler should determine block member offsets using HLSL
        /// packing rules instead of standard GLSL rules.  Defaults to false.  Only
        /// affects GLSL compilation.  HLSL rules are always used when compiling HLSL.
        /// </summary>
        public bool HlslOffsets
        {
            set => Native.shaderc_compile_options_set_hlsl_offsets(handle, value);
        }



        /// <summary>
        /// Sets whether the compiler should enable extension
        /// SPV_GOOGLE_hlsl_functionality1.
        /// </summary>
        public bool HlslFunctionality1
        {
            set => Native.shaderc_compile_options_set_hlsl_functionality1(handle, value);
        }


        /// <summary>
        /// Sets whether the compiler should invert position.Y output in vertex shader.
        /// </summary>
        public bool InvertY 
        { 
            set => Native.shaderc_compile_options_set_invert_y(handle, value); 
        }



        /// <summary>
        /// Sets whether the compiler generates code for max and min builtins which,
        /// if given a NaN operand, will return the other operand. Similarly, the clamp
        /// builtin will favour the non-NaN operands, as if clamp were implemented
        /// as a composition of max and min.
        /// </summary>
        public bool NanClamp
        {
            set => Native.shaderc_compile_options_set_nan_clamp(handle, value);
        }



        /// <summary>
        /// Sets the target SPIR-V version. The generated module will use this version
        /// of SPIR-V.  Each target environment determines what versions of SPIR-V
        /// it can consume.  Defaults to the highest version of SPIR-V 1.0 which is
        /// required to be supported by the target environment.  E.g. Default to SPIR-V
        /// 1.0 for Vulkan 1.0 and SPIR-V 1.3 for Vulkan 1.1.
        /// </summary>
        public SpirVVersion TargetSpirVVersion
        {
            set => Native.shaderc_compile_options_set_target_spirv(handle, value);
        }


        /// <summary>
        /// Include resolution method. Override it to provide a custom include resolution. Note that
        /// this Options instance must have been created with enableIncludes set to 'true'.
        /// </summary>
        /// <returns><c>true</c>, if find include was tryed, <c>false</c> otherwise.</returns>
        /// <param name="sourcePath">requesting source name</param>
        /// <param name="includePath">include name to search for.</param>
        /// <param name="incType">As in c, relative include or global</param>
        /// <param name="incFile">the resolved name of the include, empty if resolution failed</param>
        /// <param name="incContent">if resolution succeeded, contain the source code in plain text of the include</param>
        protected virtual bool TryFindInclude(string sourcePath, string includePath, IncludeType incType, out string incFile, out string incContent)
        {
            if (incType is IncludeType.Relative)
            {
                incFile = Path.Combine(Path.GetDirectoryName(sourcePath), includePath);

                if (File.Exists(incFile))
                {
                    incContent = File.ReadAllText(incFile);

                    return true;
                }

            }
            else
            {
                foreach (string incDir in IncludeDirectories)
                {
                    incFile = Path.Combine(incDir, includePath);

                    if (File.Exists(incFile))
                    {
                        incContent = File.ReadAllText(incFile);

                        return true;
                    }
                }
            }

            incFile = string.Empty;
            incContent = incFile;

            return false;
        }

        private static IntPtr HandlePFN_IncludeResolve(IntPtr userData, string requestedSource, int type, string requestingSource, UIntPtr includeDepth)
        {

            Options opts = optionsDic[userData.ToInt32()];

            if (opts.TryFindInclude(requestingSource, requestedSource, (IncludeType)type, out string incFile, out string content))
                content = File.ReadAllText(incFile);


            IncludeResult result = new IncludeResult(incFile, content, userData.ToInt32());

            IntPtr irPtr = Interop.Alloc<IncludeResult>();

            Marshal.StructureToPtr(result, irPtr, true);


            return irPtr;
        }

        private static void HandlePFN_IncludeResultRelease(IntPtr userData, IntPtr includeResult)
        {
            Marshal.PtrToStructure<IncludeResult>(includeResult).FreeStrings();
            Marshal.FreeHGlobal(includeResult);
        }

        private void setIncludeCallbacks()
        {
            Native.shaderc_compile_options_set_include_callbacks(handle, Marshal.GetFunctionPointerForDelegate(resolve), Marshal.GetFunctionPointerForDelegate(incResultRelease), (IntPtr)id);
        }


        /// <summary>
        /// Returns a copy of the given shaderc Options.
        /// </summary>
        /// <returns>The clone.</returns>
        public object Clone()
        {
            return new Options(Native.shaderc_compile_options_clone(handle), includeEnabled);
        }

        /// <summary>
        /// Adds a predefined macro to the compilation options. This has the same
        /// effect as passing -Dname=value to the command-line compiler.
        /// </summary>
        /// <remarks>
        /// If value is NULL, it has the same effect as passing -Dname to the command-line
        /// compiler. If a macro definition with the same name has previously been
        /// added, the value is replaced with the new value. The macro name and
        /// value are passed in with char pointers, which point to their data, and
        /// the lengths of their data. The strings that the name and value pointers
        /// point to must remain valid for the duration of the call, but can be
        /// modified or deleted after this function has returned. In case of adding
        /// a valueless macro, the value argument should be a null pointer or the
        /// value_length should be 0u.
        /// </remarks>
        /// <param name="name">Name.</param>
        /// <param name="value">Value.</param>
        public void AddMacroDefinition(string name, string value = null)
        {
            Native.shaderc_compile_options_add_macro_definition( handle, name, (ulong)name.Length, value, string.IsNullOrEmpty(value) ? 0 : (ulong)value.Length);
        }


        /// <summary>
        /// Sets the compiler mode to generate debug information in the output.
        /// </summary>
        public void EnableDebugInfo()
        {
            Native.shaderc_compile_options_set_generate_debug_info(handle);
        }

        /// <summary>
        /// Sets the compiler mode to suppress warnings, overriding warnings-as-errors
        /// mode. When both suppress-warnings and warnings-as-errors modes are
        /// turned on, warning messages will be inhibited, and will not be emitted
        /// as error messages.
        /// </summary>
        public void DisableWarnings()
        {
            Native.shaderc_compile_options_set_suppress_warnings(handle);
        }

        /// <summary>
        /// Forces the GLSL language version and profile to a given pair. The version
        /// number is the same as would appear in the #version annotation in the source.
        /// Version and profile specified here overrides the #version annotation in the
        /// source. Use profile: 'shaderc_profile_none' for GLSL versions that do not
        /// define profiles, e.g. versions below 150.
        /// </summary>
        public void ForceVersionAndProfile(int version, Profile profile)
        {
            Native.shaderc_compile_options_set_forced_version_profile(handle, version, profile);
        }

        /// <summary>
        /// Sets the target shader environment, affecting which warnings or errors will
        /// be issued.  The version will be for distinguishing between different versions
        /// of the target environment.  The version value should be either 0 or
        /// a value listed in shaderc_env_version.  The 0 value maps to Vulkan 1.0 if
        /// |target| is Vulkan, and it maps to OpenGL 4.5 if |target| is OpenGL.
        /// </summary>
        public void SetTargetEnvironment(TargetEnvironment target, EnvironmentVersion version)
        {
            Native.shaderc_compile_options_set_target_env(handle, target, version);
        }


        /// <summary>
        /// Sets the compiler mode to treat all warnings as errors. Note the
        /// suppress-warnings mode overrides this option, i.e. if both
        /// warning-as-errors and suppress-warnings modes are set, warnings will not
        /// be emitted as error messages.
        /// </summary>
        public void EnableWarningsAsErrors()
        {
            Native.shaderc_compile_options_set_warnings_as_errors(handle);
        }

        /// <summary>
        /// Sets a resource limit.
        /// </summary>
        public void SetLimit(Limit limit, int value)
        {
            Native.shaderc_compile_options_set_limit(handle, limit, value);
        }


        /// <summary>
        /// Sets the base binding number used for for a uniform resource type when
        /// automatically assigning bindings.  For GLSL compilation, sets the lowest
        /// automatically assigned number.  For HLSL compilation, the regsiter number
        /// assigned to the resource is added to this specified base.
        /// </summary>
        public void SetBindingBase(UniformKind kind, uint _base)
        {
            Native.shaderc_compile_options_set_binding_base(handle, kind, _base);
        }

        /// <summary>
        /// Sets the base binding number used for for a uniform resource type when
        /// automatically assigning bindings when compiling a given shader stage.
        /// For GLSL compilation, sets the lowest automatically assigned number.  For HLSL compilation, the regsiter number
        /// assigned to the resource is added to this specified base.
        /// The stage is assumed to be one of vertex, fragment, tessellation evaluation, tesselation control, geometry, or compute.
        /// </summary>
        public void SetBindingBase(ShaderKind shaderKind, UniformKind kind, uint _base)
        {
            Native.shaderc_compile_options_set_binding_base_for_stage(handle, shaderKind, kind, _base);
        }

        /// <summary>
        /// Sets whether the compiler should automatically assign locations to
        /// uniform variables that don't have explicit locations in the shader source.
        /// </summary>
        public bool AutoMapLocations
        {
            set => Native.shaderc_compile_options_set_auto_map_locations(handle, value);
        }
        /// <summary>
        /// Sets a descriptor set and binding for an HLSL register in the given stage.
        /// This method keeps a copy of the string data.
        /// </summary>
        public void SetHlslRegisterSetAndBinding(ShaderKind shaderKind, string reg, string set, string binding)
        {
            Native.shaderc_compile_options_set_hlsl_register_set_and_binding_for_stage(handle, shaderKind, reg, set, binding);
        }

        /// <summary>
        /// Sets a descriptor set and binding for an HLSL register for all shader stages.
        /// This method keeps a copy of the string data.
        /// </summary>
        public void SetHlslRegisterSetAndBinding(string reg, string set, string binding)
        {
            Native.shaderc_compile_options_set_hlsl_register_set_and_binding(handle, reg, set, binding);
        }




        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (handle == IntPtr.Zero)
            {
                return;
            }

            if (disposing)
            {
                optionsDic.Remove(id);
            }
            else
            {
                Debug.WriteLine("[shaderc]Options disposed by finalyser");
            }

            Native.shaderc_compile_options_release(handle);
            handle = IntPtr.Zero;
        }
    }
}
