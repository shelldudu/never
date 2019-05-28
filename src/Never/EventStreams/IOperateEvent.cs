using Never.Events;
using System;

namespace Never.EventStreams
{
    /// <summary>
    /// 带操作属性的事件接口
    /// </summary>
    public interface IOperateEvent
    {
        /// <summary>
        /// 事件所在的运行域
        /// </summary>
        string AppDomain { get; }

        /// <summary>
        /// 事件
        /// </summary>
        IEvent Event { get; }

        /// <summary>
        /// 事件类型
        /// </summary>
        string EventType { get; }

        /// <summary>
        /// 事件类型全名称
        /// </summary>
        string EventTypeFullName { get; }

        /// <summary>
        /// 聚合根类型
        /// </summary>
        Type AggregateType { get; }

        /// <summary>
        /// 操作事件
        /// </summary>
        DateTime CreateDate { get; }

        /// <summary>
        /// 操作者
        /// </summary>
        string Creator { get; }

        /// <summary>
        /// 版本号
        /// </summary>
        int Version { get; }

        /// <summary>
        /// 唯一标识的Type
        /// </summary>
        Type AggregateIdType { get; }

        /// <summary>
        /// 唯一标识
        /// </summary>
        string AggregateId { get; }

        /// <summary>
        /// 当前的HashCode
        /// </summary>
        int HashCode { get; }

        /// <summary>
        /// 当前环境下的自增Id
        /// </summary>
        long Increment { get; }
    }
}