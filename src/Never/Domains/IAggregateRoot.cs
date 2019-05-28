using Never.Events;
using System;
using System.Collections.Generic;

namespace Never.Domains
{
    /// <summary>
    /// 聚合根
    /// </summary>
    /// <typeparam name="TAggregateRootId">聚合根类型</typeparam>
    public interface IAggregateRoot<TAggregateRootId> : IAggregateRoot
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        TAggregateRootId AggregateId { get; }
    }

    /// <summary>
    /// 聚合根
    /// </summary>
    public interface IAggregateRoot : IDisposable
    {
        /// <summary>
        /// 版本
        /// </summary>
        int Version { get; }

        /// <summary>
        /// 获取当前修改中递增的版本号，通常每一个事件会加一个版本号
        /// </summary>
        int IncrementVersion { get; }

        /// <summary>
        /// 获取更改的事件
        /// </summary>
        /// <returns></returns>
        IEvent[] GetChanges();

        /// <summary>
        /// 获取更新事件的条数
        /// </summary>
        /// <returns></returns>
        int GetChangeCounts();

        /// <summary>
        /// 当前情况是否可以允许提交
        /// </summary>
        /// <returns></returns>
        bool CanCommit();

        /// <summary>
        /// 更新版本并清空事件源列表
        /// </summary>
        /// <param name="version">版本</param>
        IEvent[] Change(int version);

        /// <summary>
        /// 用历史事件来重新还原聚合根最新状态
        /// </summary>
        /// <param name="history">历史领域事件</param>
        void ReplyEvent(IEnumerable<IEvent> history);
    }
}