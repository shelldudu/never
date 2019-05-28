using Never.Messages;
using System.Collections.Generic;

namespace Never.EventStreams
{
    /// <summary>
    /// 批量事件接口
    /// </summary>
    public interface IBatchEventStreamMessage : IMessage
    {
        /// <summary>
        /// 批量事件
        /// </summary>
        IEnumerable<EventStreamMessage> Messages { get; }
    }
}