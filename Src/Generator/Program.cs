// This code has been adapted from the sample repository "Vortice.Vulkan": https://github.com/amerkoleci/Vortice.Vulkan
// Copyright (c) Amer Koleci y colaboradores.
// Distribuido bajo la licencia MIT. Consulte el archivo LICENCIA en la raíz del proyecto para obtener más información.



using System;
using System.Collections.Generic;
using System.IO;
using CppAst;

namespace Generator
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            string outputPath = AppContext.BaseDirectory;
            if (args.Length > 0)
            {
                outputPath = args[0];
            }

            if (!Path.IsPathRooted(outputPath))
            {
                outputPath = Path.Combine(AppContext.BaseDirectory, outputPath);
            }

            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            string? headerFile = Path.Combine(AppContext.BaseDirectory, "spirv", "spirv_cross_c.h");
            var options = new CppParserOptions
            {
                ParseMacros = true,
            };

            var compilation = CppParser.ParseFile(headerFile, options);

            // Print diagnostic messages
            if (compilation.HasErrors)
            {
                foreach (var message in compilation.Diagnostics.Messages)
                {
                    if (message.Type == CppLogMessageType.Error)
                    {
                        var currentColor = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(message);
                        Console.ForegroundColor = currentColor;
                    }
                }

                return 0;
            }

            CsCodeGenerator.Generate(compilation, outputPath);
            return 0;
        }


    }
}
