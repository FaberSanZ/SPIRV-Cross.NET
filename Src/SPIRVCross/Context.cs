// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static SharpSPIRVCross.spvc;

namespace SharpSPIRVCross
{
    public sealed class Context : IDisposable
    {
        // references needed to prevent GC from collecting callbacks passed to native code
        private static spvc_error_callback s_ErrorCallback;
        private bool _allocationsReleased;

        public readonly IntPtr _context;

        public string LastErrorMessage
        {
            get => Marshal.PtrToStringAnsi(spvc_context_get_last_error_string(_context));
        }

        public Context()
        {
            s_ErrorCallback = ErrorCallback;

            spvc_context_create(out _context).CheckError();
            spvc_context_set_error_callback(_context, s_ErrorCallback, IntPtr.Zero);
        }

        public void Dispose()
        {
            ReleaseAllocations();
            spvc_context_destroy(_context);
        }

        public void ReleaseAllocations()
        {
            if (_allocationsReleased)
                return;

            spvc_context_release_allocations(_context);
            _allocationsReleased = true;
        }

        public ParseIr ParseIr(byte[] spirv)
        {
            unsafe
            {
                var result = spvc_context_parse_spirv(_context,
                    Unsafe.AsPointer(ref spirv[0]),
                    new IntPtr(spirv.Length / 4),
                    out var parsed_ir);
                result.CheckError();
                return new ParseIr(parsed_ir);
            }
        }

        public ParseIr ParseIr(uint[] spirv)
        {
            unsafe
            {
                var result = spvc_context_parse_spirv(_context,
                    Unsafe.AsPointer(ref spirv[0]),
                    new IntPtr(spirv.Length),
                    out var parsed_ir);
                result.CheckError();
                return new ParseIr(parsed_ir);
            }
        }

        public ParseIr ParseIr(IntPtr codePointer, uint len)
        {
            unsafe
            {
                var result = spvc_context_parse_spirv(_context,
                    (void*)codePointer,
                    new IntPtr(len),
                    out var parsed_ir);
                result.CheckError();
                return new ParseIr(parsed_ir);
            }
        }

        public Compiler CreateCompiler(Backend backend, ParseIr ir, CaptureMode mode = CaptureMode.TakeOwnership)
        {
            var result = spvc_context_create_compiler(_context, backend, ir.Handle, mode, out var compiler);
            result.CheckError();
            return new Compiler(compiler);
        }

        [MonoPInvokeCallback(typeof(spvc_error_callback))]
        private static void ErrorCallback(IntPtr userData, [MarshalAs(UnmanagedType.LPStr)] string description)
        {
            Console.WriteLine(description);
        }
    }
}
