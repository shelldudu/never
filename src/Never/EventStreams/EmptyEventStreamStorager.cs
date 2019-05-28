using Never.Commands;
using Never.Events;
using System;
using System.Collections.Generic;

namespace Never.EventStreams
{
    /// <summary>
    /// 空的事件流保存对象
    /// </summary>
    public sealed class EmptyEventStreamStorager : IEventStorager, IEventStreamStorager
    {
        #region ctor

        /// <summary>
        /// Prevents a default instance of the <see cref="EmptyEventStreamStorager"/> class from being created.
        /// </summary>
        public EmptyEventStreamStorager()
        {
        }

        #endregion ctor

        #region IEventStreamTypeStorager

        /// <summary>
        /// 批量保存领域事件
        /// </summary>
        /// <param name="events">事件列表</param>
        /// <returns></returns>
        public void Save(IEnumerable<IOperateEvent> events)
        {
        }

        /// <summary>
        /// 批量保存领域事件
        /// </summary>
        /// <param name="commandContext">命令上下文</param>
        /// <param name="events">事件列表</param>
        /// <returns></returns>
        public void Save(ICommandContext commandContext, IEnumerable<KeyValuePair<Type, IEvent[]>> events)
        {

        }

        #endregion IEventStreamTypeStorager

        #region only

        /// <summary>
        /// Gets the only.
        /// </summary>
        /// <value>
        /// The only.
        /// </value>
        public static EmptyEventStreamStorager Empty
        {
            get
            {
                if (Singleton<EmptyEventStreamStorager>.Instance == null)
                    Singleton<EmptyEventStreamStorager>.Instance = new EmptyEventStreamStorager();

                return Singleton<EmptyEventStreamStorager>.Instance;
            }
        }

        #endregion only
    }
}