using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SPIRVCross
{
    public static unsafe partial class SPIRV
    {




        internal static IntPtr LoadNativeLibrary()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return LibraryLoader.LoadLocalLibrary("spirv-cross-c-shared.dll");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return LibraryLoader.LoadLocalLibrary("spirv-cross-c-shared.so");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return LibraryLoader.LoadLocalLibrary("spirv-cross-c-shared.dylib");
            }
            else
            {
                return LibraryLoader.LoadLocalLibrary("spirv-cross-c-shared");
            }
        }
    }



    internal static class LibraryLoader
    {
        static LibraryLoader()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Extension = ".dll";
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                Extension = ".dylib";
            else
                Extension = ".so";
        }

        public static string Extension { get; }

        public static IntPtr LoadLocalLibrary(string libraryName)
        {

            var libWithExt = libraryName;
            if (!libraryName.EndsWith(Extension, StringComparison.OrdinalIgnoreCase))
                libWithExt += Extension;

            return NativeLibrary.Load(libWithExt);

        }

        public static T LoadFunction<T>(IntPtr library, string name)
        {

            IntPtr symbol = NativeLibrary.GetExport(library, name);

            if (symbol == IntPtr.Zero)
                throw new EntryPointNotFoundException($"Unable to load symbol '{name}'.");

            return Marshal.GetDelegateForFunctionPointer<T>(symbol);
        }
    }
}
