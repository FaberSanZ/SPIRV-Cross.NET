using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Shaderc
{

    // An includer callback type for mapping an #include request to an include
    // result.  The user_data parameter specifies the client context.  The
    // requested_source parameter specifies the name of the source being requested.
    // The type parameter specifies the kind of inclusion request being made.
    // The requesting_source parameter specifies the name of the source containing
    // the #include request.  The includer owns the result object and its contents,
    // and both must remain valid until the release callback is called on the result
    // object.
    internal delegate IntPtr PFN_IncludeResolve(IntPtr userData, string requestedSource, int type, string requestingSource, UIntPtr includeDepth);
    // An includer callback type for destroying an include result.
    internal delegate void PFN_IncludeResultRelease(IntPtr userData, IntPtr includeResult);


    /// <summary>
    /// An include result.
    /// </summary>
    public unsafe struct IncludeResult
    {
        private readonly byte* sourceName;
        private readonly UIntPtr sourceNameLength;
        private readonly byte* content;
        private readonly UIntPtr contentLength;
        /// <summary>
        /// User data to be passed along with this request.
        /// </summary>
        public readonly IntPtr UserData;
        /// <summary>
        /// The name of the source file.  The name should be fully resolved
        /// in the sense that it should be a unique name in the context of the
        /// includer.  For example, if the includer maps source names to files in
        /// a filesystem, then this name should be the absolute path of the file.
        /// For a failed inclusion, this string is empty.
        /// </summary>
        public string SourceName => Interop.String.FromPointer(sourceName);
        /// <summary>
        /// The text contents of the source file in the normal case.
        /// For a failed inclusion, this contains the error message.
        /// </summary>
        public string Content => Interop.String.FromPointer(content);

        internal IncludeResult(string sourceName, string content, int optionsId)
        {
            this.sourceName = Interop.String.ToPointer(sourceName);
            sourceNameLength = (UIntPtr)sourceName.Length;
            this.content = Interop.String.ToPointer(content);
            contentLength = (UIntPtr)content.Length;
            UserData = (IntPtr)optionsId;
        }
        internal void FreeStrings()
        {
            // TODO: Free Strings
            Marshal.FreeHGlobal((IntPtr)sourceName);
            Marshal.FreeHGlobal((IntPtr)content);
        }
    }
}
