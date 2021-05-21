// This code has been based from the sample repository "Vortice.Vulkan": https://github.com/amerkoleci/Vortice.Vulkan
// Copyright (c) Amer Koleci and contributors.
// Copyright (c) 2020 - 2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)




using System.Collections.Generic;
using System.IO;
using System.Text;
using CppAst;

namespace Generator
{
    public static partial class CsCodeGenerator
    {
        private static readonly HashSet<string> s_instanceFunctions = new HashSet<string>
        {

        };

        private static readonly HashSet<string> s_outReturnFunctions = new HashSet<string>
        {

        };


        private static void GenerateCommands(CppCompilation compilation, string outputPath)
        {
            // Generate Functions
            using var writer = new CodeWriter(Path.Combine(outputPath, "Commands.cs"),
                "System",
                "System.Runtime.InteropServices"
                );

            var commands = new Dictionary<string, CppFunction>();
            var instanceCommands = new Dictionary<string, CppFunction>();
            var deviceCommands = new Dictionary<string, CppFunction>();
            foreach (CppFunction? cppFunction in compilation.Functions)
            {
                string? returnType = GetCsTypeName(cppFunction.ReturnType, false);
                bool canUseOut = s_outReturnFunctions.Contains(cppFunction.Name);
                string? csName = cppFunction.Name;

                commands.Add(csName, cppFunction);

                if (cppFunction.Parameters.Count > 0)
                {
                    var firstParameter = cppFunction.Parameters[0];
                    if (firstParameter.Type is CppTypedef typedef)
                    {


                        deviceCommands.Add(csName, cppFunction);
                        
                    }
                }
            }

            using (writer.PushBlock($"public unsafe partial class SPIRV"))
            {
                writer.WriteLine("internal static IntPtr s_NativeLibrary = LoadNativeLibrary();");
                writer.WriteLine("internal static T LoadFunction<T>(string name) => LibraryLoader.LoadFunction<T>(s_NativeLibrary, name);");

                foreach (KeyValuePair<string, CppFunction> command in commands)
                {
                    CppFunction cppFunction = command.Value;


                    string returnCsName = GetCsTypeName(cppFunction.ReturnType, false);
                    bool canUseOut = s_outReturnFunctions.Contains(cppFunction.Name);
                    var argumentsString = GetParameterSignature(cppFunction, canUseOut);

                    writer.WriteLine("[UnmanagedFunctionPointer(CallingConvention.Cdecl)]");
                    writer.WriteLine($"private delegate {returnCsName}  PFN_{cppFunction.Name}({argumentsString});");
                    //        private static readonly PFN_shaderc_compile_options_set_suppress_warnings shaderc_compile_options_set_suppress_warnings_ = LoadFunction<PFN_shaderc_compile_options_set_suppress_warnings>("shaderc_compile_options_set_suppress_warnings");
                    writer.WriteLine($"private static readonly PFN_{cppFunction.Name} {cppFunction.Name}_ = LoadFunction<PFN_{cppFunction.Name}>(nameof({cppFunction.Name}));");

                    using (writer.PushBlock($"public static {returnCsName} {cppFunction.Name}({argumentsString})"))
                    {
                        if (returnCsName != "void")
                        {
                            writer.Write("return ");
                        }

                        writer.Write($"{command.Key}_(");
                        int index = 0;
                        foreach (CppParameter cppParameter in cppFunction.Parameters)
                        {
                            string paramCsName = GetParameterName(cppParameter.Name);

                            if (canUseOut && CanBeUsedAsOutput(cppParameter.Type, out CppTypeDeclaration? cppTypeDeclaration))
                            {
                                writer.Write("out ");
                            }

                            writer.Write($"{paramCsName}");

                            if (index < cppFunction.Parameters.Count - 1)
                            {
                                writer.Write(", ");
                            }

                            index++;
                        }

                        writer.WriteLine(");");
                    }

                    writer.WriteLine();
                }
            }
        }





        public static string GetParameterSignature(CppFunction cppFunction, bool canUseOut)
        {
            return GetParameterSignature(cppFunction.Parameters, canUseOut);
        }

        private static string GetParameterSignature(IList<CppParameter> parameters, bool canUseOut)
        {
            var argumentBuilder = new StringBuilder();
            int index = 0;

            foreach (CppParameter cppParameter in parameters)
            {
                string direction = string.Empty;
                var paramCsTypeName = GetCsTypeName(cppParameter.Type, false);
                var paramCsName = GetParameterName(cppParameter.Name);

                if (canUseOut && CanBeUsedAsOutput(cppParameter.Type, out CppTypeDeclaration? cppTypeDeclaration))
                {
                    argumentBuilder.Append("out ");
                    paramCsTypeName = GetCsTypeName(cppTypeDeclaration, false);
                }

                argumentBuilder.Append(paramCsTypeName).Append(' ').Append(paramCsName);
                if (index < parameters.Count - 1)
                {
                    argumentBuilder.Append(", ");
                }

                index++;
            }

            return argumentBuilder.ToString();
        }

        private static string GetParameterName(string name)
        {
            if (name == "event")
                return "@event";

            if (name == "object")
                return "@object";

            if (name.StartsWith('p')
                && char.IsUpper(name[1]))
            {
                name = char.ToLower(name[1]) + name.Substring(2);
                return GetParameterName(name);
            }

            return name;
        }

        private static bool CanBeUsedAsOutput(CppType type, out CppTypeDeclaration? elementTypeDeclaration)
        {
            if (type is CppPointerType pointerType)
            {
                if (pointerType.ElementType is CppTypedef typedef)
                {
                    elementTypeDeclaration = typedef;
                    return true;
                }
                else if (pointerType.ElementType is CppClass @class
                    && @class.ClassKind != CppClassKind.Class
                    && @class.SizeOf > 0)
                {
                    elementTypeDeclaration = @class;
                    return true;
                }
                else if (pointerType.ElementType is CppEnum @enum
                    && @enum.SizeOf > 0)
                {
                    elementTypeDeclaration = @enum;
                    return true;
                }
            }

            elementTypeDeclaration = null;
            return false;
        }
    }
}
