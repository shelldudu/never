using Never.Messages;
using System;

namespace Never.EventStreams
{
    /// <summary>
    /// 领域事件消息
    /// </summary>
    [Serializable]
    public sealed class EventStreamMessage : IMessage
    {
        /// <summary>
        /// 事件所在的运行域
        /// </summary>
        public string AppDomain { get; set; }

        /// <summary>
        /// 操作事件
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 操作者
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// 事件类型
        /// </summary>
        public string EventType { get; set; }

        /// <summary>
        /// 事件名字
        /// </summary>
        public string EventName { get; set; }

        /// <summary>
        /// 唯一标识
        /// </summary>
        public string AggregateId { get; set; }

        /// <summary>
        /// 唯一标识的Type
        /// </summary>
        public string AggregateIdType { get; set; }

        /// <summary>
        /// 聚合根Type
        /// </summary>
        public string AggregateType { get; set; }

        /// <summary>
        /// 事件内容
        /// </summary>
        public string EventContent { get; set; }

        /// <summary>
        /// 当前的HashCode
        /// </summary>
        public int HashCode { get; set; }

        /// <summary>
        /// 当前环境的自增Id
        /// </summary>
        public long Increment { get; set; }
    }
}