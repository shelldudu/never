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
    public interface ISocketBuffer
    {
        /// <summary>
        /// 缓存区
        /// </summary>
        ArraySegment<byte> Segment { get; }

        /// <summary>
        /// 所在组
        /// </summary>
        RangeTuple<int, int> Group { get; }
    }
}
