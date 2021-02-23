// Copyright (c) Amer Koleci and contributors.
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static SharpSPIRVCross.spvc;

namespace SharpSPIRVCross
{
    public sealed class SpirvType
    {
        private readonly Compiler _compiler;
        internal readonly IntPtr Handle;

        public readonly SpirvBaseType BaseType;
        public readonly int BitWidth;
        public readonly int VectorSize;
        public readonly int Columns;
        public readonly int ArrayDimensions;
        public readonly int MemberCount;
        public readonly SpvStorageClass StorageClass;

        public readonly SpirvType ImageSampledType;
        public readonly SpvDim ImageDimension;
        public readonly bool ImageIsDepth;
        public readonly bool ImageIsArray;
        public readonly bool ImageIsMultisampled;
        public readonly bool ImageIsStorage;
        public readonly SpvImageFormat ImageStorageFormat;
        public readonly SpvAccessQualifier ImageAccessQualifier;

        internal SpirvType(Compiler compiler, IntPtr handle)
        {
            _compiler = compiler;
            Handle = handle;
            BaseType = spvc_type_get_basetype(handle);
            BitWidth = spvc_type_get_bit_width(handle);
            VectorSize = spvc_type_get_vector_size(handle);
            Columns = spvc_type_get_columns(handle);
            ArrayDimensions = spvc_type_get_num_array_dimensions(handle);
            MemberCount = spvc_type_get_num_member_types(handle);
            StorageClass = spvc_type_get_storage_class(handle);
            ImageSampledType = compiler.GetSpirvType(spvc_type_get_image_sampled_type(handle));
            if (ImageSampledType != null)
            {
                ImageDimension = spvc_type_get_image_dimension(handle);
                ImageIsDepth = spvc_type_get_image_is_depth(handle);
                ImageIsArray = spvc_type_get_image_arrayed(handle);
                ImageIsMultisampled = spvc_type_get_image_multisampled(handle);
                ImageIsStorage = spvc_type_get_image_is_storage(handle);
                ImageStorageFormat = spvc_type_get_image_storage_format(handle);
                ImageAccessQualifier = spvc_type_get_image_access_qualifier(handle);
            }
        }

        public uint GetArrayDimension(int dimension)
        {
            return spvc_type_get_array_dimension(Handle, dimension);
        }

        public bool ArrayDimensionIsLiteral(int dimension)
        {
            return spvc_type_array_dimension_is_literal(Handle, dimension);
        }

        public SpirvType GetMemberType(int index)
        {
            var handle = spvc_type_get_member_type(Handle, index);
            if (handle == IntPtr.Zero)
                return null;

            return new SpirvType(_compiler, handle);
        }

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        private static extern SpirvBaseType spvc_type_get_basetype(IntPtr type);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        private static extern int spvc_type_get_bit_width(IntPtr type);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        private static extern int spvc_type_get_vector_size(IntPtr type);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        private static extern int spvc_type_get_columns(IntPtr type);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        private static extern int spvc_type_get_num_array_dimensions(IntPtr type);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool spvc_type_array_dimension_is_literal(IntPtr type, int dimension);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint spvc_type_get_array_dimension(IntPtr type, int dimension);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        private static extern int spvc_type_get_num_member_types(IntPtr type);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr spvc_type_get_member_type(IntPtr type, int index);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        private static extern SpvStorageClass spvc_type_get_storage_class(IntPtr type);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        private static extern uint spvc_type_get_image_sampled_type(IntPtr type);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        private static extern SpvDim spvc_type_get_image_dimension(IntPtr type);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool spvc_type_get_image_is_depth(IntPtr type);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool spvc_type_get_image_arrayed(IntPtr type);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool spvc_type_get_image_multisampled(IntPtr type);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool spvc_type_get_image_is_storage(IntPtr type);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        private static extern SpvImageFormat spvc_type_get_image_storage_format(IntPtr type);

        [DllImport("cspirv_cross", CallingConvention = CallingConvention.Cdecl)]
        private static extern SpvAccessQualifier spvc_type_get_image_access_qualifier(IntPtr type);
    }

    public enum SpirvBaseType
    {
        Unknown = 0,
        Void = 1,
        Boolean = 2,
        Int8 = 3,
        UInt8 = 4,
        Int16 = 5,
        UInt16 = 6,
        Int32 = 7,
        UInt32 = 8,
        Int64 = 9,
        UInt64 = 10,
        AtomicCounter = 11,
        Fp16 = 12,
        Fp32 = 13,
        Fp64 = 14,
        Struct = 15,
        Image = 16,
        SampledImage = 17,
        Sampler = 18,
        AccelerationStructure = 19
    }
}
