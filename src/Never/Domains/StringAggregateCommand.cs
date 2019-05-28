using System;

namespace Never.Domains
{
    /// <summary>
    /// 聚合根命令
    /// </summary>
    [Serializable, Never.CommandStreams.IgnoreStoreCommand, Never.Attributes.IgnoreAnalyse]
    public class StringAggregateCommand : IAggregateCommand<string>
    {
        #region prop

        /// <summary>
        /// 版本
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// 唯一标识
        /// </summary>
        public string AggregateId { get; protected set; }

        #endregion prop

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="StringAggregateCommand"/> class.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        protected StringAggregateCommand(string aggregateId)
        {
            this.AggregateId = aggregateId;
        }

        #endregion ctor
    }
}