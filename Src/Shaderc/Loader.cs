// Copyright (c) 2020 - 2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*===================================================================================
	Interop.cs
====================================================================================*/



using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;


namespace Shaderc
{
    internal static unsafe class Interop
    {
        public static TDelegate GetDelegateForFunctionPointer<TDelegate>(IntPtr pointer) => Marshal.GetDelegateForFunctionPointer<TDelegate>(pointer);
        public static IntPtr GetFunctionPointerForDelegate<TDelegate>(TDelegate @delegate) => Marshal.GetFunctionPointerForDelegate(@delegate);


        public static IntPtr Alloc(int byteCount)
        {
            if (byteCount is 0)
            {
                return IntPtr.Zero;
            }

            return Marshal.AllocHGlobal(byteCount);
        }

        public static IntPtr Alloc<T>(int count = 1)
        {
            return Alloc(Unsafe.SizeOf<T>() * count);
        }


        public static class String
        {

            public static byte* ToPointer(string value)
            {
                return (byte*)(void*)AllocToPointer(value);
            }

            public static string FromPointer(IntPtr pointer)
            {
                return FromPointer((byte*)(void*)pointer);
            }

            public static string FromPointer(byte* pointer)
            {
                if (pointer is null)
                {
                    return string.Empty;
                }

                // Read until null-terminator.
                byte* walkPtr = pointer;
                while (*walkPtr != 0)
                {
                    walkPtr++;
                }

                // Decode UTF-8 bytes to string.
                return Encoding.UTF8.GetString(pointer, (int)(walkPtr - pointer));
            }


            public static IntPtr AllocToPointer(string value)
            {
                if (value is null)
                {
                    return IntPtr.Zero;
                }

                // Get max number of bytes the string may need.
                int maxSize = GetMaxByteCount(value);

                // Allocate unmanaged memory.
                IntPtr managedPtr = Alloc(maxSize);
                byte* ptr = (byte*)managedPtr;

                // Encode to utf-8, null-terminate and write to unmanaged memory.
                int actualNumberOfBytesWritten;
                fixed (char* ch = value)
                {
                    actualNumberOfBytesWritten = Encoding.UTF8.GetBytes(ch, value.Length, ptr, maxSize);
                }

                ptr[actualNumberOfBytesWritten] = 0;

                // Return pointer to the beginning of unmanaged memory.
                return managedPtr;
            }

            public static byte** AllocToPointers(string[] values)
            {
                if (values == null || values.Length == 0)
                {
                    return null;
                }

                // Allocate unmanaged memory for string pointers.
                byte** stringHandlesPtr = (byte**)(void*)Alloc<IntPtr>(values.Length);

                for (int i = 0; i < values.Length; i++)
                {
                    // Store the pointer to the string.
                    stringHandlesPtr[i] = (byte*)AllocToPointer(values[i]);
                }

                return stringHandlesPtr;
            }

            public static int GetMaxByteCount(string value)
            {
                return value is null ? 0 : Encoding.UTF8.GetMaxByteCount(value.Length + 1);
            }
        }

    }
    // LibShader
    public static class Loader
    {
        private static readonly IntPtr _handle;
        private const int LibDLRtldNow = 2;


        static Loader()
        {
            _handle = GetShaderLibraryNameCandidates().Select(LoadLibrary).FirstOrDefault(handle => handle != IntPtr.Zero);

            if (_handle == IntPtr.Zero)
                throw new NotImplementedException("Shaderc native library was not found.");
        }

        public static TDelegate GetStaticProc<TDelegate>(string procName) where TDelegate : class
        {
            IntPtr handle;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                handle = Kernel32GetProcAddress(_handle, procName);

            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                handle = LibDLGetProcAddress(_handle, procName);

            else
                throw new NotImplementedException();

            return handle == IntPtr.Zero ? null : Interop.GetDelegateForFunctionPointer<TDelegate>(handle);
        }

        private static IntPtr LoadLibrary(string fileName)
        {
            IntPtr handle;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                handle = Kernel32LoadLibrary(fileName);

            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                handle = LibDLLoadLibrary(fileName, LibDLRtldNow);

            else
                throw new NotImplementedException();

            return handle;
        }

        private static IEnumerable<string> GetShaderLibraryNameCandidates()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                yield return "shaderc_shared.dll";

            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                yield return ""; // Known to be present on Ubuntu 16.
            }

            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                yield return "libshaderc_shared.so"; // Using on MacOS.

            throw new NotImplementedException("Ran out of places to look for the Shaderc native library");
        }


        [DllImport("kernel32", EntryPoint = "LoadLibrary")]
        private static extern IntPtr Kernel32LoadLibrary(string fileName);


        [DllImport("kernel32", EntryPoint = "GetProcAddress")]
        private static extern IntPtr Kernel32GetProcAddress(IntPtr module, string procName);


        [DllImport("libdl", EntryPoint = "dlopen")]
        private static extern IntPtr LibDLLoadLibrary(string fileName, int flags);


        [DllImport("libdl", EntryPoint = "dlsym")]
        private static extern IntPtr LibDLGetProcAddress(IntPtr handle, string name);
    }
}
