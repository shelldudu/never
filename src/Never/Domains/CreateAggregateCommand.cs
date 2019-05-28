using System;

namespace Never.Domains
{
    /// <summary>
    /// 创建聚合根命令
    /// </summary>
    [Serializable, Never.CommandStreams.IgnoreStoreCommand, Never.Attributes.IgnoreAnalyse]
    public class CreateAggregateCommand<TAggregateRootId> : IAggregateCommand<TAggregateRootId>
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
        /// Initializes a new instance of the <see cref="CreateAggregateCommand{TAggregateRootId}"/> class.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        protected CreateAggregateCommand(TAggregateRootId aggregateId)
        {
            this.AggregateId = aggregateId;
        }

        #endregion ctor
    }
}