using Never.IoC;
using System.Collections.Generic;

namespace Never.Events
{
    /// <summary>
    /// 事件提供者
    /// </summary>
    public class EventHandlerProvider
    {
        #region field

        /// <summary>
        /// 服务定位器
        /// </summary>
        private readonly ILifetimeScope scope = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerProvider"/> class.
        /// </summary>
        public EventHandlerProvider(ILifetimeScope scope)
        {
            this.scope = scope;
        }

        #endregion ctor

        #region prop

        /// <summary>
        ///
        /// </summary>
        public ILifetimeScope Scope
        {
            get
            {
                return this.scope;
            }
        }

        #endregion prop

        #region IEventProvider成员

        /// <summary>
        /// 查找所有监听了T类型的事件监听者
        /// </summary>
        /// <typeparam name="TEvent">领域事件类型</typeparam>
        /// <returns></returns>
        public IEnumerable<IEventHandler<TEvent>> FindHandler<TEvent>() where TEvent : IEvent
        {
            return this.scope.ResolveAll<IEventHandler<TEvent>>();
        }

        /// <summary>
        /// 获取事件上下文提供者
        /// </summary>
        /// <returns></returns>
        public IEventContext FindEventContext()
        {
            return this.scope.ResolveOptional<IEventContext>() ?? new DefaultEventContext();
        }

        #endregion IEventProvider成员
    }
}