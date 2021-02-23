using System;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using SharpSPIRVCross;
using Vortice.ShaderCompiler;

namespace SPIRVCross.Test
{
    internal class Program
    {
        private static void Main(string[] args)
        {


            Options _options = new();
            _options.SetSourceLanguage(SourceLanguage.GLSL);

            using Vortice.ShaderCompiler.Compiler compilerfile = new(_options);

            var result = compilerfile.Compile(File.ReadAllText("Shaders/lighting.frag"), string.Empty, ShaderKind.VertexShader);

            byte[] bytecode = result.GetBytecode().ToArray();

            using (Context context = new Context())
            {
                //Assert.NotNull(context);
                ParseIr ir = context.ParseIr(bytecode);
                SharpSPIRVCross.Compiler compiler = context.CreateCompiler(Backend.GLSL, ir);

                SpvCapability[] caps = compiler.GetDeclaredCapabilities();
                string[] extensions = compiler.GetDeclaredExtensions();
                ShaderResources resources = compiler.CreateShaderResources();

                Console.WriteLine(compiler.ExecutionModel);

                foreach (ReflectedResource uniformBuffer in resources.GetResources(ResourceType.PushConstant))
                {
                    Console.WriteLine($"ID: {uniformBuffer.Id}, BaseTypeID: {uniformBuffer.BaseTypeId}, TypeID: {uniformBuffer.TypeId}, Name: {uniformBuffer.Name})");
                    uint set = compiler.GetDecoration(uniformBuffer.Id, SpvDecoration.DescriptorSet);
                    uint binding = compiler.GetDecoration(uniformBuffer.Id, SpvDecoration.Binding);
                    int size = 0;
                    SpirvType type = compiler.GetSpirvType(uniformBuffer.TypeId);
                    compiler.GetDeclaredStructSize(type, out size);
                    Console.WriteLine($"  Set: {set}, Binding: {binding}, size {size} = {Marshal.SizeOf<Matrix4x4>()}");
                }


                foreach (ReflectedResource uniformBuffer in resources.GetResources(ResourceType.SubpassInput))
                {
                    Console.WriteLine($"ID: {uniformBuffer.Id}, BaseTypeID: {uniformBuffer.BaseTypeId}, TypeID: {uniformBuffer.TypeId}, Name: {uniformBuffer.Name})");
                    uint set = compiler.GetDecoration(uniformBuffer.Id, SpvDecoration.DescriptorSet);
                    uint binding = compiler.GetDecoration(uniformBuffer.Id, SpvDecoration.Binding);
                    Console.WriteLine($"  Set: {set}, Binding: {binding}");
                }


                foreach (ReflectedResource uniformBuffer in resources.GetResources(ResourceType.UniformBuffer))
                {
                    Console.WriteLine($"ID: {uniformBuffer.Id}, BaseTypeID: {uniformBuffer.BaseTypeId}, TypeID: {uniformBuffer.TypeId}, Name: {uniformBuffer.Name})");
                    uint set = compiler.GetDecoration(uniformBuffer.Id, SpvDecoration.DescriptorSet);
                    uint binding = compiler.GetDecoration(uniformBuffer.Id, SpvDecoration.Binding);
                    Console.WriteLine($"  Set: {set}, Binding: {binding}");
                }





                foreach (ReflectedResource input in resources.GetResources(ResourceType.StageInput))
                {
                    Console.WriteLine($"ID: {input.Id}, BaseTypeID: {input.BaseTypeId}, TypeID: {input.TypeId}, Name: {input.Name})");
                    uint location = compiler.GetDecoration(input.Id, SpvDecoration.Location);
                    Console.WriteLine($"  Location: {location}");
                }

                foreach (ReflectedResource sampledImage in resources.GetResources(ResourceType.SampledImage))
                {

                    Console.WriteLine($"ID: {sampledImage.Id}, BaseTypeID: {sampledImage.BaseTypeId}, TypeID: {sampledImage.TypeId}, Name: {sampledImage.Name})");

                    uint set = compiler.GetDecoration(sampledImage.Id, SpvDecoration.DescriptorSet);
                    uint binding = compiler.GetDecoration(sampledImage.Id, SpvDecoration.Binding);
                    SpirvType type = compiler.GetSpirvType(sampledImage.TypeId);
                    

                    Console.WriteLine($"  Set: {set}, Binding: {binding}");
                }

                compiler.Options.SetOption(CompilerOption.HLSL_ShaderModel, 50);
                string hlsl_source = compiler.Compile();
            }
        }
    }
}
