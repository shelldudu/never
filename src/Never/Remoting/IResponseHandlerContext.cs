using Never;
using Never.Sockets.AsyncArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Remoting
{
    /// <summary>
    /// 上下文
    /// </summary>
    public interface IResponseHandlerContext
    {
        /// <summary>
        /// socket事件
        /// </summary>
        SocketEventArgs EventArgs { get; }
    }
}
