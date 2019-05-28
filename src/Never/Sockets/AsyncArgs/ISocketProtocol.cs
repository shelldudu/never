using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Sockets.AsyncArgs
{
    /// <summary>
    /// 协议
    /// </summary>
    public interface ISocketProtocol
    {
        /// <summary>
        /// 将date转成发送的数据
        /// </summary>
        /// <param name="data">原始数据</param>
        /// <returns></returns>
        byte[] To(byte[] data);

        /// <summary>
        /// 将接受到的数据转成目标数据
        /// </summary>
        /// <param name="collection">接受的目标数据</param>
        /// <returns></returns>
        byte[] From(ConcurrentQueue<byte[]> collection);
    }
}
