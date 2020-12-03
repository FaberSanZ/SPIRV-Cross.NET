// Copyright (c) 2020 - 2021 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*===================================================================================
	Result.cs
====================================================================================*/


using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Shaderc
{
    public unsafe class Result : IDisposable
    {
        internal IntPtr handle;

        public Result()
        {

        }

        public Result(IntPtr handle)
        {
            this.handle = handle;
            if (handle == IntPtr.Zero)
            {
                throw new Exception("error");
            }
        }


        /// <summary>
        /// Returns the compilation status, indicating whether the compilation succeeded,
        /// or failed due to some reasons, like invalid shader stage or compilation
        /// errors.
        /// </summary>
        public Status Status => Native.shaderc_result_get_compilation_status(handle);
        /// <summary>
        /// Returns the number of errors generated during the compilation.
        /// </summary>
        /// 

        public uint ErrorCount => (uint)Native.shaderc_result_get_num_errors(handle);
        /// <summary>
        /// // Returns the number of warnings generated during the compilation.
        /// </summary>
        



        public uint WarningCount => (uint)Native.shaderc_result_get_num_warnings(handle);
        
        
        
        /// <summary>
        /// Returns a null-terminated string that contains any error messages generated
        /// during the compilation.
        /// </summary>
        public string ErrorMessage => Marshal.PtrToStringAnsi(Native.shaderc_result_get_error_message(handle));

        
        
        /// <summary>
        /// Returns a pointer to the start of the compilation output data bytes, either
        /// SPIR-V binary or char string. When the source string is compiled into SPIR-V
        /// binary, this is guaranteed to be castable to a uint32_t*. If the result
        /// contains assembly text or preprocessed source text, the pointer will point to
        /// the resulting array of characters.
        /// </summary>
        public IntPtr CodePointer => new IntPtr(Native.shaderc_result_get_bytes(handle));
        
        
        
        /// <summary>
        /// Returns the number of bytes of the compilation output data in a result object.
        /// </summary>
        public ulong CodeLength => Native.shaderc_result_get_length(handle);



        public byte GetDataPointer(int index)
        {
            return Native.shaderc_result_get_bytes(handle)[index];
        }


        public byte* GetDataPointer()
        {
            return Native.shaderc_result_get_bytes(handle);
        }


        public byte[] GetData()
        {

            byte[] result = new byte[CodeLength];

            if (CodePointer != IntPtr.Zero)
                Marshal.Copy(CodePointer, result, 0, result.Length);


            return result;
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (handle == IntPtr.Zero)
                return;

            if (!disposing)
                Debug.WriteLine("[shaderc]Result disposed by finalyser");

            Native.shaderc_result_release(handle);

            handle = IntPtr.Zero;
        }
    }
}
