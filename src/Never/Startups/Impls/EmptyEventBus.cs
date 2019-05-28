using Never.Commands;
using Never.Events;
using System.Collections.Generic;

namespace Never.Startups.Impls
{
    /// <summary>
    /// 没有任何功能的命令总线
    /// </summary>
    public sealed class EmptyEventBus : IEventBus
    {
        #region field

        /// <summary>
        /// 空对象
        /// </summary>
        public static EmptyEventBus Only
        {
            get
            {
                if (Singleton<EmptyEventBus>.Instance == null)
                    Singleton<EmptyEventBus>.Instance = new EmptyEventBus();

                return Singleton<EmptyEventBus>.Instance;
            }
        }

        #endregion field

        #region ctor

        /// <summary>
        /// Prevents a default instance of the <see cref="EmptyEventBus"/> class from being created.
        /// </summary>
        private EmptyEventBus()
        {
        }

        #endregion ctor

        void IEventBus.Push(ICommandContext context, IEnumerable<IEvent[]> events)
        {
        }

        void IEventBus.Publish<TEvent>(ICommandContext commandContext, TEvent e)
        {
        }
    }
}
