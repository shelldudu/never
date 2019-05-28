using System;

namespace Never.Domains
{
    /// <summary>
    /// 更新创建根Id
    /// </summary>
    [Serializable, Never.CommandStreams.IgnoreStoreCommand, Never.Attributes.IgnoreAnalyse]
    public class ChangeAggregateCommand<TAggregateRootId> : IAggregateCommand<TAggregateRootId>
    {
        #region prop

        /// <summary>
        /// 版本
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// 唯一标识
        /// </summary>
        public TAggregateRootId AggregateId { get; protected set; }

        #endregion prop

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeAggregateCommand{TAggregateRootId}"/> class.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        protected ChangeAggregateCommand(TAggregateRootId aggregateId)
        {
            this.AggregateId = aggregateId;
        }

        #endregion ctor
    }
}