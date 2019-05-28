using System;

namespace Never.Events
{
    /// <summary>
    /// 聚合根事件
    /// </summary>
    /// <typeparam name="TAggregateRootKey">聚合根唯一标识对象的类型</typeparam>
    public interface IAggregateRootEvent<TAggregateRootKey> : IEvent
    {
        #region property

        /// <summary>
        /// 唯一标识
        /// </summary>
        TAggregateRootKey AggregateId { get; }

        /// <summary>
        /// 操作时间
        /// </summary>
        DateTime CreateDate { get; }

        /// <summary>
        /// 操作者
        /// </summary>
        string Creator { get; }

        #endregion property
    }
}