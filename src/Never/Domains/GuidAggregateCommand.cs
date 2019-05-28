using Never.Utils;
using System;

namespace Never.Domains
{
    /// <summary>
    /// 聚合根命令
    /// </summary>
    [Serializable, Never.CommandStreams.IgnoreStoreCommand, Never.Attributes.IgnoreAnalyse]
    public class GuidAggregateCommand : IAggregateCommand<Guid>
    {
        #region prop

        /// <summary>
        /// 版本
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// 唯一标识
        /// </summary>
        public Guid AggregateId { get; protected set; }

        #endregion prop

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="GuidAggregateCommand"/> class.
        /// </summary>
        /// <param name="aggregateId">The aggregate identifier.</param>
        protected GuidAggregateCommand(Guid aggregateId)
        {
            this.AggregateId = aggregateId;
        }

        #endregion ctor

        #region new

        /// <summary>
        /// 创建一个干净的命令
        /// </summary>
        /// <returns></returns>
        public static GuidAggregateCommand New()
        {
            return new GuidAggregateCommand(NewId.GenerateGuid());
        }

        /// <summary>
        /// 创建一个干净的命令
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static GuidAggregateCommand New(Guid guid)
        {
            return new GuidAggregateCommand(guid);
        }

        #endregion new
    }
}