using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Never.Sockets.AsyncArgs
{
    /// <summary>
    /// 连接对象
    /// </summary>
    public interface IConnection : IDisposable
    {
        /// <summary>
        /// 会话Id
        /// </summary>
        ulong Id { get; }

        /// <summary>
        /// 获取本机终结点
        /// </summary>
        EndPoint LocalEndPoint { get; }

        /// <summary>
        /// 获取远程终结点
        /// </summary>
        EndPoint RemoteEndPoint { get; }

        /// <summary>
        /// 连接时间
        /// </summary>
        DateTime ConnectTime { get; }

        /// <summary>
        /// 支持的协议
        /// </summary>
        ProtocolType ProtocolType { get; }

        /// <summary>
        /// 是否连接了
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// 获取会话是否提供SSL/TLS安全
        /// </summary>
        bool IsSecurity { get; }
    }
}
