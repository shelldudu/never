using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Memcached.BinaryProtocols
{
    /// <summary>
    /// 字节序特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class EndianAttribute : Attribute
    {
        /// <summary>
        /// 标记字段的字节序
        /// </summary>
        public Endian Endianness { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="endianness">字节序</param>
        public EndianAttribute(Endian endianness)
        {
            this.Endianness = endianness;
        }
    }
}
