using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Never.Sockets.AsyncArgs
{
    /// <summary>
    /// socket事件
    /// </summary>
    public class SocketEventArgs : EventArgs
    {
        /// <summary>
        /// 会话
        /// </summary>
        public IConnection Connection { get; }

        /// <summary>
        /// 
        /// </summary>
        public SocketEventArgs(IConnection connection)
        {
            this.Connection = connection;
        }
    }

    /// <summary>
    /// 消息到达后的事件
    /// </summary>
    public class OnReceivedSocketEventArgs : SocketEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public byte[] Buffer { get; }

        /// <summary>
        /// 
        /// </summary>
        public OnReceivedSocketEventArgs(IConnection connection, byte[] buffer) : base(connection)
        {
            this.Buffer = buffer;
        }
    }


    /// <summary>
    /// socket关闭达后的事件
    /// </summary>
    public class OnClosedSocketEventArgs : SocketEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public SocketError Error { get; }

        /// <summary>
        /// 异常信息
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// 
        /// </summary>
        public OnClosedSocketEventArgs(IConnection connection, SocketError error, Exception exception) : base(connection)
        {
            this.Error = error;
            this.Exception = exception;
        }
    }
}
