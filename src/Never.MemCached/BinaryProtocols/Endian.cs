using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Memcached.BinaryProtocols
{

    /// <summary>
    /// 字节序枚举
    /// </summary>
    public enum Endian
    {
        /// <summary>
        /// 大端字节序
        /// </summary>
        BigEndian,

        /// <summary>
        /// 小端字节序
        /// </summary>
        LittleEndian
    }

}
