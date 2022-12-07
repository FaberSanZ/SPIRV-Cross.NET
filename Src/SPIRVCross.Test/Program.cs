using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SPIRVCross;
using static SPIRVCross.SPIRV;
using Vortice.ShaderCompiler;
using System.Text;
using Vortice.Dxc;

namespace SPIRVCross.Test
{
    internal class Program
    {
        private static unsafe void Main(string[] args)
        {

            string GetString(byte* ptr)
            {
                int length = 0;
                while (length < 4096 && ptr[length] != 0)
                    length++;
                // Decode UTF-8 bytes to string.
                return Encoding.UTF8.GetString(ptr, length);
            }


            Options _options = new();
            _options.SetSourceLanguage(SourceLanguage.GLSL);

            using Compiler compilerfile = new(_options);

            var result = compilerfile.Compile(File.ReadAllText("Shaders/lighting.frag"), string.Empty, ShaderKind.VertexShader);

            //Span<byte> bytecode = result.GetBytecode();
            byte[] bytecode = CompileLibraryShader("Shaders/lighting.hlsl");

            SpvId* spirv;

            fixed (byte* ptr = bytecode)
                spirv = (SpvId*)ptr;

            uint word_count = (uint)bytecode.Length / 4;



            spvc_context context = default;
            spvc_parsed_ir ir;
            spvc_compiler compiler_glsl;
            spvc_compiler_options options;
            spvc_resources resources;
            spvc_reflected_resource* list = default;
            byte* result_ = null;
            nuint count = default;
            spvc_error_callback error_callback = default;


            // Create context.
            spvc_context_create(&context);

            // Set debug callback.
            spvc_context_set_error_callback(context, error_callback, null);

            // Parse the SPIR-V.
            spvc_context_parse_spirv(context, spirv, word_count, &ir);

            // Hand it off to a compiler instance and give it ownership of the IR.
            spvc_context_create_compiler(context, spvc_backend.Hlsl, ir, spvc_capture_mode.TakeOwnership, &compiler_glsl);

            // Do some basic reflection.
            spvc_compiler_create_shader_resources(compiler_glsl, &resources);
            spvc_resources_get_resource_list_for_type(resources, spvc_resource_type.PushConstant, (spvc_reflected_resource*)&list, &count);


            var model = spvc_compiler_get_execution_model(compiler_glsl);

            Console.WriteLine(model);

            for (uint i = 0; i < count; i++)
            {
                uint set = spvc_compiler_get_decoration(compiler_glsl, (SpvId)list[i].id, SpvDecoration.SpvDecorationDescriptorSet);
                uint binding = spvc_compiler_get_decoration(compiler_glsl, (SpvId)list[i].id, SpvDecoration.SpvDecorationBinding);
                uint offset = spvc_compiler_get_decoration(compiler_glsl, (SpvId)list[i].id, SpvDecoration.SpvDecorationOffset);
                spvc_type type = spvc_compiler_get_type_handle(compiler_glsl, (SpvId)list[i].type_id);

                nuint size = 0;
                spvc_compiler_get_declared_struct_size(compiler_glsl, type, &size);


                Console.WriteLine(size);


                Console.WriteLine("=========");
            }
            Console.WriteLine("\n \n");
            //spvc_

            // Modify options.
            spvc_compiler_create_compiler_options(compiler_glsl, &options);
            //spvc_compiler_options_set_uint(options, spvc_compiler_option.HlslShaderModel, 51);
            spvc_compiler_options_set_bool(options, spvc_compiler_option.HlslShaderModel, true);
            spvc_compiler_install_compiler_options(compiler_glsl, options);


            //byte* r = null;
            //spvc_compiler_compile(compiler_glsl, (byte*)&r);
            //Console.WriteLine("Cross-compiled source: {0}", GetString(r));

            // Frees all memory we allocated so far.
            spvc_context_destroy(context);
        }



        private static byte[] CompileLibraryShader(string filePath)
        {
            string? source = File.ReadAllText(filePath);
            var profile = "hs_6_5";
            string[] args = new[]
            {
                "-spirv",
                "-T", profile,
                "-E", "main",
                "-fspv-target-env=vulkan1.2",
                "-fspv-extension=SPV_NV_ray_tracing",
                "-fspv-extension=SPV_KHR_multiview",
                "-fspv-extension=SPV_KHR_shader_draw_parameters",
                "-fspv-extension=SPV_EXT_descriptor_indexing",

            };
            IDxcUtils? utils = Dxc.CreateDxcUtils();
            IDxcIncludeHandler? handler = utils!.CreateDefaultIncludeHandler();

            var compiler = Dxc.CreateDxcCompiler3();

            IDxcResult? result = compiler?.Compile(source, args, handler);

            if (result == null || result.GetStatus().Failure)
            {
                throw new Exception(result!.GetErrors());
            }

            byte[] data = result.GetObjectBytecodeArray();

            result.Dispose();
            return data;
        }
    }
}
