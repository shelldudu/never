using System;

namespace Never.Events
{
    /// <summary>
    /// 聚合根删除事件
    /// </summary>
    /// <typeparam name="TAggregateRootId">聚合根唯一标识对象的类型</typeparam>
    [Serializable, Never.EventStreams.IgnoreStoreEventAttribute, Never.Attributes.IgnoreAnalyse]
    public class AggregateRootDeleteEvent<TAggregateRootId> : IAggregateRootEvent<TAggregateRootId>
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
        /// 版本
        /// </summary>
        public int Version { get; set; }

        #endregion property

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRootDeleteEvent{TAggregateRootId}"/> class.
        /// </summary>
        public AggregateRootDeleteEvent()
            : this(default(TAggregateRootId), "sys")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRootDeleteEvent{TAggregateRootId}"/> class.
        /// </summary>
        /// <param name="uniqueId">The unique identifier.</param>
        /// <param name="editor">The creator.</param>
        protected AggregateRootDeleteEvent(TAggregateRootId uniqueId, string editor)
        {
            this.AggregateId = uniqueId;
            this.Version = 0;
            this.CreateDate = DateTime.Now;
            this.Creator = string.IsNullOrEmpty(editor) ? "sys" : editor;
        }

        #endregion ctor
    }
}