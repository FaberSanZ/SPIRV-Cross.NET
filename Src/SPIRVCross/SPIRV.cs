using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SPIRVCross
{

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
