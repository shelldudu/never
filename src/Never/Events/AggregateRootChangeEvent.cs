using System;

namespace Never.Events
{
    /// <summary>
    /// 聚合根更新事件
    /// </summary>
    /// <typeparam name="TAggregateRootId">聚合根唯一标识对象的类型</typeparam>
    [Serializable, Never.EventStreams.IgnoreStoreEventAttribute, Never.Attributes.IgnoreAnalyse]
    public class AggregateRootChangeEvent<TAggregateRootId> : IAggregateRootEvent<TAggregateRootId>
    {
        #region property

        /// <summary>
        /// 唯一标识
        /// </summary>
        public TAggregateRootId AggregateId { get; set; }

        /// <summary>
        /// 编辑时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 编辑者
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public int Version { get; set; }

        #endregion property

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRootChangeEvent{TAggregateRootId}"/> class.
        /// </summary>
        public AggregateRootChangeEvent()
            : this(default(TAggregateRootId), "sys")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRootChangeEvent{TAggregateRootId}"/> class.
        /// </summary>
        /// <param name="uniqueId">The unique identifier.</param>
        /// <param name="creator">The creator.</param>
        protected AggregateRootChangeEvent(TAggregateRootId uniqueId, string creator)
        {
            this.AggregateId = uniqueId;
            this.Version = 0;
            this.CreateDate = DateTime.Now;
            this.Creator = string.IsNullOrEmpty(creator) ? "sys" : creator;
        }

        #endregion ctor
    }
}