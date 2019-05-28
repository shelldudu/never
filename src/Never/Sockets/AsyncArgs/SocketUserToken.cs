using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Sockets.AsyncArgs
{
    /// <summary>
    /// token
    /// </summary>
    public class SocketUserToken
    {
        /// <summary>
        /// 当前缓冲区信息
        /// </summary>
        public ISocketBuffer SocketBuffer { get; set; }
    }
}
