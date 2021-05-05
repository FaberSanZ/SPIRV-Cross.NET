// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

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

        private static string GetFunctionPointerSignature(bool netstandard, CppFunction function, bool allowNonBlittable = true)
        {
            bool canUseOut = s_outReturnFunctions.Contains(function.Name);

            StringBuilder builder = new();
            foreach (CppParameter parameter in function.Parameters)
            {
                string paramCsType = GetCsTypeName(parameter.Type, false);

                if (canUseOut &&
                    CanBeUsedAsOutput(parameter.Type, out CppTypeDeclaration? cppTypeDeclaration))
                {
                    builder.Append("out ");
                    paramCsType = GetCsTypeName(cppTypeDeclaration, false);
                }

                builder.Append(paramCsType).Append(", ");
            }

            string returnCsName = GetCsTypeName(function.ReturnType, false);
            if (!allowNonBlittable)
            {

            }

            builder.Append(returnCsName);

            if (netstandard)
            {
                return $"delegate* unmanaged[Stdcall]<{builder}>";
            }

            return $"delegate* unmanaged<{builder}>";
        }

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
                string? argumentsString = GetParameterSignature(cppFunction, canUseOut);

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
                foreach (KeyValuePair<string, CppFunction> command in commands)
                {
                    CppFunction cppFunction = command.Value;


                    string functionPointerSignatureNS = GetFunctionPointerSignature(true, cppFunction);
                    string functionPointerSignature = GetFunctionPointerSignature(false, cppFunction);



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
