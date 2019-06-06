using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Never;
using Never.Serialization;

namespace Never.Memcached
{
    public class BinaryCompressProtocol : DefaultCompressProtocol, ICompressProtocol
    {
        #region field and ctor

        private readonly IBinarySerializer binarySerializer = null;

        /// <summary>
        /// 
        /// </summary>
        public BinaryCompressProtocol() : this(new BinarySerializer(), null) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="encoding">对value为string类型的直接使用Encoding.GetBytes</param>
        public BinaryCompressProtocol(Encoding encoding) : this(new BinarySerializer(), encoding) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binarySerializer"></param>
        /// <param name="encoding">对value为string类型的直接使用Encoding.GetBytes</param>
        public BinaryCompressProtocol(IBinarySerializer binarySerializer, Encoding encoding)
        {
            this.binarySerializer = binarySerializer;
            this.Encoding = encoding;
        }

        #endregion

        #region prop

        /// <summary>
        /// 在进行string变成byte的时候可使用Encoding.GetBytes方法
        /// </summary>
        public Encoding Encoding { get; }

        #endregion

        #region icompressprotocol

        /// <summary>
        /// 压缩
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="object">对象</param>
        /// <param name="flag">flag，由于<see cref="TargetTypeFlag"/>使用了0-13的数值，但又不确定是否被子类调用，因此要么确定不使用<see cref="TargetTypeFlag"/>要么避开0-33</param>
        /// <returns></returns>
        public override byte[] Compress<T>(T @object, out byte flag)
        {
            var type = typeof(T);
            if (this.IsTargetType(type, out var @enum))
            {
                if (this.TryCompressTargetTypeFlag(@object, @enum, out var value, out flag))
                {
                    return value;
                }
            }
            if (this.IsNullableTargetType(type, out @enum))
            {
                if (this.TryCompressNullableTargetTypeFlag(@object, @enum, out var value, out flag))
                {
                    return value;
                }
            }

            if (this.Encoding != null)
            {
                if (type == typeof(string))
                {
                    flag = 22;
                    return this.Encoding.GetBytes(@object as string);
                }
                else if (type == typeof(Guid))
                {
                    flag = 23;
                    return this.Encoding.GetBytes(@object.ToString());
                }
            }

            return this.Clothing(this.binarySerializer.Serialize(@object), out flag);
        }

        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="object">对象</param>
        /// <param name="flag">flag，由于<see cref="TargetTypeFlag"/>使用了0-13的数值，但又不确定是否被子类调用，因此要么确定不使用<see cref="TargetTypeFlag"/>要么避开0-33</param>
        /// <returns></returns>
        public override byte[] Compress(object @object, out byte flag)
        {
            var type = @object.GetType();
            if (this.IsTargetType(type, out var @enum))
            {
                if (this.TryCompressTargetTypeFlag(@object, @enum, out var value, out flag))
                {
                    return value;
                }
            }

            if (this.IsNullableTargetType(type, out @enum))
            {
                if (this.TryCompressNullableTargetTypeFlag(@object, @enum, out var value, out flag))
                {
                    return value;
                }
            }

            if (this.Encoding != null)
            {
                if (type == typeof(string))
                {
                    flag = 22;
                    return this.Encoding.GetBytes(@object as string);
                }
                else if (type == typeof(Guid))
                {
                    flag = 23;
                    return this.Encoding.GetBytes(@object.ToString());
                }
            }

            return this.Clothing(this.binarySerializer.Serialize(@object), out flag);
        }

        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="byte">源数据</param>
        /// <param name="flag">flag，由于<see cref="TargetTypeFlag"/>使用了0-13的数值，但又不确定是否被子类调用，因此要么确定不使用<see cref="TargetTypeFlag"/>要么避开0-33</param>
        /// <returns></returns>
        public override bool TryDecompress(byte[] @byte, byte flag, out object @object)
        {
            @object = null;
            if (this.IsTargetType(flag, out TargetTypeFlag @enum))
            {
                if (this.TryDecompressTargetTypeFlag(@byte, flag, @enum, out @object))
                    return true;
            }

            if (this.Encoding != null)
            {
                try
                {
                    switch (flag)
                    {
                        case 22:
                            {
                                @object = this.Encoding.GetString(@byte);
                                return true;
                            }
                        case 23:
                            {
                                var @string = this.Encoding.GetString(@byte);
                                if (Guid.TryParse(@string, out var @guid))
                                {
                                    @object = guid;
                                    return true;
                                }

                                return false;
                            }
                    }
                }
                catch
                {

                }
            }

            try
            {
                @object = this.binarySerializer.DeserializeObject(this.Undressing(@byte, flag), null);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 解压缩
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="byte">源数据</param>
        /// <param name="flag">flag，由于<see cref="TargetTypeFlag"/>使用了0-13的数值，但又不确定是否被子类调用，因此要么确定不使用<see cref="TargetTypeFlag"/>要么避开0-33</param>
        /// <returns></returns>
        public override bool TryDecompress<T>(byte[] @byte, byte flag, out T @object)
        {
            @object = default(T);
            if (this.IsTargetType(flag, out TargetTypeFlag @enum))
            {
                if (this.TryDecompressTargetTypeFlag(@byte, flag, @enum, out @object))
                    return true;
            }

            if (this.Encoding != null)
            {
                try
                {
                    switch (flag)
                    {
                        case 22:
                            {
                                if (typeof(T) == typeof(string))
                                {
                                    var @string = this.Encoding.GetString(@byte);
                                    @object = (T)(dynamic)@string;
                                    return true;
                                }

                                return false;
                            }
                        case 23:
                            {
                                if (typeof(T) == typeof(Guid))
                                {
                                    var @string = this.Encoding.GetString(@byte);
                                    if (Guid.TryParse(@string, out var guid))
                                    {
                                        @object = (T)(dynamic)guid;
                                        return true;
                                    }
                                }

                                return false;

                            }
                    }
                }
                catch
                {

                }
            }

            try
            {
                @object = this.binarySerializer.Deserialize<T>(this.Undressing(@byte, flag));
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 对数据加一层信息，穿上衣服
        /// </summary>
        /// <param name="byte"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        protected virtual byte[] Clothing(byte[] @byte, out byte flag)
        {
            flag = 24;
            return @byte;
        }

        /// <summary>
        /// 对数据脱一层信息，脱衣服
        /// </summary>
        /// <param name="byte"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        protected virtual byte[] Undressing(byte[] @byte, byte flag)
        {
            if (flag == 24)
                return @byte;

            return null;
        }

        #endregion

        #region dispose

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion
    }
}
