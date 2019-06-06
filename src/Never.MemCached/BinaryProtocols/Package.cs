using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Never.Memcached.BinaryProtocols
{

    /// <summary>
    /// 
    /// </summary>
    public struct Package
    {
        private static Action<byte[]> requestReverse;
        private static Action<byte[]> responseReverse;

        public static Action<byte[]> RequestReverse = new Action<byte[]>((x) =>
        {
            if (requestReverse != null)
            {
                requestReverse(x);
                return;
            }

            var emit = Never.Reflection.EasyEmitBuilder<Action<byte[]>>.NewDynamicMethod("BinaryRequestHeaderReverse");
            var members = typeof(RequestHeader).GetMembers(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach (var member in members.Where(ta => ta.MemberType == System.Reflection.MemberTypes.Field).Cast<System.Reflection.FieldInfo>())
            {
                var attribute = member.GetAttribute<EndianAttribute>();
                if (attribute == null)
                    continue;

                var @offsetInptr = Marshal.OffsetOf(typeof(RequestHeader), member.Name);
                var @offset = @offsetInptr.ToInt32();
                var @sizeof = Marshal.SizeOf(member.FieldType);

                switch (attribute.Endianness)
                {
                    case Endian.BigEndian:
                        {
                            if (BitConverter.IsLittleEndian == false)
                                continue;

                            emit.LoadArgument(0);
                            emit.LoadConstant(@offset);
                            emit.LoadConstant(@sizeof);
                            emit.Call(typeof(Array).GetMethod("Reverse", new[] { typeof(Array), typeof(int), typeof(int) }));
                        }
                        break;
                    case Endian.LittleEndian:
                        {
                            if (BitConverter.IsLittleEndian == true)
                                continue;

                            emit.LoadArgument(0);
                            emit.LoadConstant(@offset);
                            emit.LoadConstant(@sizeof);
                            emit.Call(typeof(Array).GetMethod("Reverse", new[] { typeof(Array), typeof(int), typeof(int) }));
                        }
                        break;
                }
            }

            emit.Return();
            requestReverse = emit.CreateDelegate();
            requestReverse(x);
            return;
        });
        public static Action<byte[]> ResponseReverse = new Action<byte[]>((x) =>
        {
            if (responseReverse != null)
            {
                responseReverse(x);
                return;
            }

            var emit = Never.Reflection.EasyEmitBuilder<Action<byte[]>>.NewDynamicMethod("BinaryResponseHeaderReverse");
            var members = typeof(ResponseHeader).GetMembers(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach (var member in members.Where(ta => ta.MemberType == System.Reflection.MemberTypes.Field).Cast<System.Reflection.FieldInfo>())
            {
                var attribute = member.GetAttribute<EndianAttribute>();
                if (attribute == null)
                    continue;

                var @offsetInptr = Marshal.OffsetOf(typeof(ResponseHeader), member.Name);
                var @offset = @offsetInptr.ToInt32();
                var @sizeof = Marshal.SizeOf(member.FieldType);

                switch (attribute.Endianness)
                {
                    case Endian.BigEndian:
                        {
                            if (BitConverter.IsLittleEndian == false)
                                continue;

                            emit.LoadArgument(0);
                            emit.LoadConstant(@offset);
                            emit.LoadConstant(@sizeof);
                            emit.Call(typeof(Array).GetMethod("Reverse", new[] { typeof(Array), typeof(int), typeof(int) }));
                        }
                        break;
                    case Endian.LittleEndian:
                        {
                            if (BitConverter.IsLittleEndian == true)
                                continue;

                            emit.LoadArgument(0);
                            emit.LoadConstant(@offset);
                            emit.LoadConstant(@sizeof);
                            emit.Call(typeof(Array).GetMethod("Reverse", new[] { typeof(Array), typeof(int), typeof(int) }));
                        }
                        break;
                }
            }

            emit.Return();
            responseReverse = emit.CreateDelegate();
            responseReverse(x);
            return;
        });

        public static unsafe KeyValuePair<RequestHeader, IEnumerable<byte[]>> AddRequest(Command command, string key, byte[] body, byte flag, int expireTime)
        {
            var keyValue = Encoding.ASCII.GetBytes(key);
            var extra = new byte[8];
            Buffer.BlockCopy(BitConverter.GetBytes((uint)flag), 0, extra, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes((uint)expireTime), 0, extra, 4, 4);
            var request = new RequestHeader()
            {
                Magic = Magic.Request,
                OpCode = command,
                KeyLength = (ushort)keyValue.Length,
                ExtraLength = (byte)extra.Length,
                DataType = 0,
                VbucketId = 0,
                TotalBody = extra.Length + keyValue.Length + body.Length,
                Opaque = 0,
                CAS = 0,
            };

            var @reqs = request.ToByte();
            RequestReverse(reqs);
            var @enumerable = new[] { @reqs, extra, keyValue, body }.ToArray();
            return new KeyValuePair<RequestHeader, IEnumerable<byte[]>>(request, @enumerable);
        }

        public static KeyValuePair<RequestHeader, IEnumerable<byte[]>> DeleteRequest(string key)
        {
            var keyValue = Encoding.ASCII.GetBytes(key);
            var request = new RequestHeader()
            {
                OpCode = Command.delete,
                DataType = 0,
                Magic = Magic.Request,
                ExtraLength = 0,
                KeyLength = (ushort)keyValue.Length,
                TotalBody = keyValue.Length
            };

            var @enumerable = new[] { request.ToByte(), keyValue };
            return new KeyValuePair<RequestHeader, IEnumerable<byte[]>>(request, @enumerable);
        }

        public static KeyValuePair<RequestHeader, IEnumerable<byte[]>> GetRequest(string key)
        {
            var keyValue = Encoding.ASCII.GetBytes(key);
            var request = new RequestHeader()
            {
                Magic = Magic.Request,
                OpCode = Command.get,
                KeyLength = (ushort)keyValue.Length,
                ExtraLength = 0,
                DataType = 0,
                VbucketId = 0,
                TotalBody = keyValue.Length,
                Opaque = 0,
                CAS = 0,
            };

            var @reqs = request.ToByte();
            RequestReverse(reqs);
            var @enumerable = new[] { @reqs, keyValue };
            return new KeyValuePair<RequestHeader, IEnumerable<byte[]>>(request, @enumerable);
        }

        public static KeyValuePair<RequestHeader, IEnumerable<byte[]>> InterlockedRequest(Command command, string key, long inter, int expireTime)
        {
            var keyValue = Encoding.ASCII.GetBytes(key);
            var extra = new byte[20];
            Buffer.BlockCopy(BitConverter.GetBytes(inter), 0, extra, 0, 8);
            Buffer.BlockCopy(BitConverter.GetBytes(0L), 0, extra, 8, 8);
            Buffer.BlockCopy(BitConverter.GetBytes(expireTime), 0, extra, 16, 4);
            var request = new RequestHeader()
            {
                Magic = Magic.Request,
                OpCode = command,
                KeyLength = (ushort)keyValue.Length,
                ExtraLength = (byte)extra.Length,
                DataType = 0,
                VbucketId = 0,
                TotalBody = extra.Length + keyValue.Length,
                Opaque = 0,
                CAS = 0,
            };

            var @reqs = request.ToByte();
            RequestReverse(reqs);
            var @enumerable = new[] { @reqs, extra, keyValue };
            return new KeyValuePair<RequestHeader, IEnumerable<byte[]>>(request, @enumerable);
        }
    }
}
