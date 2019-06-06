using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Memcached.BinaryProtocols
{
    /// <summary>
    /// 
    /// </summary>
    public enum Magic : byte
    {
        /// <summary>
        /// 请求
        /// </summary>
        Request = 0x80,

        /// <summary>
        /// 响应
        /// </summary>
        Response = 0x81,
    }
}
