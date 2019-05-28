using Never.Events;
using System;

namespace Never.EventStreams
{
    /// <summary>
    /// 带操作属性的事件
    /// </summary>
    public class DefaultOperateEvent : IOperateEvent
    {
        #region prop

        /// <summary>
        /// 事件所在的运行域
        /// </summary>
        public string AppDomain { get; set; }

        /// <summary>
        /// 事件
        /// </summary>
        public IEvent Event { get; set; }

        /// <summary>
        /// 操作事件
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 操作者
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// 聚合根类型
        /// </summary>
        public Type AggregateType { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// 唯一标识的Type
        /// </summary>
        public Type AggregateIdType { get; set; }

        /// <summary>
        /// 事件类型
        /// </summary>
        public string EventType { get; set; }

        /// <summary>
        /// 事件类型全名称
        /// </summary>
        public string EventTypeFullName { get; set; }

        /// <summary>
        /// 唯一标识
        /// </summary>
        public string AggregateId { get; set; }

        /// <summary>
        /// 当前的HashCode
        /// </summary>
        public int HashCode { get; set; }

        /// <summary>
        /// 当前环境的自增Id
        /// </summary>
        public long Increment { get; set; }

        #endregion prop

        #region incerement

        /// <summary>
        /// 自增数值
        /// </summary>
        private static long increment = 0;

        /// <summary>
        /// 获取自增数值
        /// </summary>
        public static long NextIncrement
        {
            get
            {
                return System.Threading.Interlocked.Increment(ref increment);
            }
        }

        #endregion incerement
    }
}