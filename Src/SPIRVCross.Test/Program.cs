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

namespace SPIRVCross.Test
{
    internal class Program
    {
        private static unsafe void Main(string[] args)
        {

            static int GetMaxByteCount(string? value)
            {
                return value == null
                    ? 0
                    : Encoding.UTF8.GetMaxByteCount(value.Length + 1); // +1 for null-terminator.
            }

            string GetString(byte* ptr)
            {
                int length = 0;
                while (length < 4096 && ptr[length] != 0)
                {
                    length++;
                }

                // Decode UTF-8 bytes to string.
                return Encoding.UTF8.GetString(ptr, length);
            }



            Options _options = new();
            _options.SetSourceLanguage(SourceLanguage.GLSL);

            using Vortice.ShaderCompiler.Compiler compilerfile = new(_options);

            var result = compilerfile.Compile(File.ReadAllText("Shaders/lighting.frag"), string.Empty, ShaderKind.FragmentShader);

            byte[] bytecode = result.GetBytecode().ToArray();

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
            uint i;


            // Create context.
            spvc_context_create(&context);


            spvc_error_callback error_callback = default;
            // Set debug callback.
            spvc_context_set_error_callback(context, error_callback, null);

            // Parse the SPIR-V.
            spvc_context_parse_spirv(context, spirv, word_count, &ir);
            // Hand it off to a compiler instance and give it ownership of the IR.
            spvc_context_create_compiler(context, spvc_backend.Glsl, ir, spvc_capture_mode.TakeOwnership, &compiler_glsl);

            // Do some basic reflection.
            spvc_compiler_create_shader_resources(compiler_glsl, &resources);
            spvc_resources_get_resource_list_for_type(resources, spvc_resource_type.UniformBuffer, (spvc_reflected_resource*)&list, &count);

            for (i = 0; i < count; i++)
            {
                Console.WriteLine("ID: {0}, BaseTypeID: {1}, TypeID: {2}, Name: {3}", list[i].id, list[i].base_type_id, list[i].type_id, GetString(list[i].name));

                uint set = spvc_compiler_get_decoration(compiler_glsl, (SpvId)list[i].id, SpvDecoration.SpvDecorationDescriptorSet);
                Console.WriteLine($"Set: {set}");

                uint binding = spvc_compiler_get_decoration(compiler_glsl, (SpvId)list[i].id, SpvDecoration.SpvDecorationBinding);
                Console.WriteLine($"Binding: {binding}");


                Console.WriteLine("=========");
            }


        }
    }
}
