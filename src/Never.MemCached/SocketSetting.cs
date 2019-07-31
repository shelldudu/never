using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Memcached
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
        /// 连接池中最大个socket活动
        /// </summary>
        public int MaxPoolBufferSize { get; set; } = 10;

        /// <summary>
        /// 心跳时间
        /// </summary>
        public TimeSpan KeepAlivePeriod { get; set; } = TimeSpan.FromMinutes(10);
    }
}
