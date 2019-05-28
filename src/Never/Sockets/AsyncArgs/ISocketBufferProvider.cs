using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Sockets.AsyncArgs
{
    /// <summary>
    /// 缓冲区
    /// </summary>
    public interface ISocketBufferProvider
    {
        /// <summary>
        /// 获取一个缓冲区
        /// </summary>
        /// <returns></returns>
        ISocketBuffer Alloc();

        /// <summary>
        /// 回收
        /// </summary>
        /// <param name="item"></param>
        void Recycle(ISocketBuffer item);
    }
}
