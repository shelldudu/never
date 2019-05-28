using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Sockets.AsyncArgs
{
    /// <summary>
    /// 
    /// </summary>
    public class SocketSetting
    {
        /// <summary>
        /// socket接受数据缓存区大小(8k)
        /// </summary>
        public int ReceiveBufferSize { get; set; } = 1024 * 8;

        /// <summary>
        /// socket发送数据缓存区大小(8k)
        /// </summary>
        public int SendBufferSize { get; set; } = 1024 * 8;

        /// <summary>
        /// 服务器最大连接
        /// </summary>
        public int MaxBacklog { get; set; } = 100;

        /// <summary>
        /// 心跳间隔
        /// </summary>
        public TimeSpan KeepAlivePeriod { get; set; } = TimeSpan.FromSeconds(1);
    }
}
