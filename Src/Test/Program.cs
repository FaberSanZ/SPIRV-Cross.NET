using Shaderc;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Test
{
    unsafe class Program
    {

        static void Main(string[] args)
        {
            Options options = new Options();
            options.SourceLanguage = SourceLanguage.Glsl;


            using Compiler compiler = new Compiler(options);

            Result res = compiler.Compile(@"shaders/debug.vert", ShaderKind.VertexShader);

            for (int i = 0; i < (int)res.CodeLength; i++)
            {
                Console.Write(res.GetData()[i]);
                Console.Write(" - ");
                Console.Write(res.GetDataPointer(i));
                Console.WriteLine();
            }


            //foreach (var d in res.GetData())
            //    Console.WriteLine(d);

        }
    }
}
