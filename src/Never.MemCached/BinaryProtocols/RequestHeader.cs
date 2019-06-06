using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Never.Memcached.BinaryProtocols
{

    /// <summary>
    /// header
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct RequestHeader
    {
        [FieldOffset(0)]
        public Magic Magic;

        [FieldOffset(1)]
        public Command OpCode;

        [FieldOffset(2), EndianAttribute(Endian.BigEndian)]
        public ushort KeyLength;

        [FieldOffset(4)]
        public byte ExtraLength;

        [FieldOffset(5)]
        public byte DataType;

        [FieldOffset(6), EndianAttribute(Endian.BigEndian)]
        public short VbucketId;

        [FieldOffset(8), EndianAttribute(Endian.BigEndian)]
        public int TotalBody;

        [FieldOffset(12), EndianAttribute(Endian.BigEndian)]
        public int Opaque;

        [FieldOffset(16), EndianAttribute(Endian.BigEndian)]
        public long CAS;

        /// <summary>
        /// 获取大小
        /// </summary>
        /// <returns></returns>
        public static int Size()
        {
            return System.Runtime.InteropServices.Marshal.SizeOf(typeof(RequestHeader));
        }
    }
}
