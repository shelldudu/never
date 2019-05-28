using Never.Aop.DomainFilters;
using Never.Commands;
using Never.IoC;
using Never.Logging;
using System.Collections.Generic;

namespace Never.Events
{
    /// <summary>
    /// 事件扩展
    /// </summary>
    public static class EventBusExcutingHelper
    {
        #region element

        internal static List<EventExcutingElement> FindEventExcutingElement<TEvent>(EventHandlerProvider provider, ICommandContext commandContext, TEvent e) where TEvent : IEvent
        {
            var eventHandlers = provider.FindHandler<TEvent>();
            if (eventHandlers == null)
                return new List<EventExcutingElement> { };

            /*先获取到每个eventHandler的所有属性*/
            var elements = new List<EventExcutingElement>();
            foreach (var item in eventHandlers)
            {
                var handlerType = item.GetType();
                var attributes = HandlerBehaviorStorager.Default.GetAttributes(handlerType);
                var sortAttribute = ObjectExtension.GetAttribute<EventHandlerPriorityAttribute>(attributes);
                var loggerAttribute = ObjectExtension.GetAttribute<Aop.LoggerAttribute>(attributes);
                ILoggerBuilder loggerBuilder = null;
                if (loggerAttribute != null)
                {
                    try
                    {
                        loggerBuilder = loggerAttribute.RegisterKey.IsNotNullOrEmpty() ? provider.Scope.Resolve<ILoggerBuilder>(loggerAttribute.RegisterKey) : provider.Scope.ResolveOptional<ILoggerBuilder>();
                    }
                    catch
                    {
                    }
                }

                var eventContext = provider.FindEventContext();
                if (eventContext is DefaultEventContext)
                {
                    ((DefaultEventContext)eventContext).WorkTime = eventContext.WorkTime;
                }

                elements.Add(new EventExcutingElement()
                {
                    Order = (sortAttribute == null || sortAttribute.Order < 0) ? 1 : sortAttribute.Order,
                    EventContext = eventContext,
                    EventHandler = item,
                    EventHandlerType = handlerType,
                    LoggerBuilder = loggerBuilder ?? LoggerBuilder.Empty,
                    LoggerAttribute = loggerAttribute,
                    HandlerFilters = ObjectExtension.GetAttributes<EventHandlerFilterAttribute>(HandlerBehaviorStorager.Default.GetAttributes(handlerType, e.GetType()))
                });
            }

            elements.Sort((x, y) =>
            {
                if (x.Equals(y))
                    return 1;
                return x.Order > y.Order ? 1 : -1;
            });

            return elements;
        }

        #endregion element

        #region runtime

        /// <summary>
        /// 新加运行时类型到上下文
        /// </summary>
        /// <param name="eventBus">事件总线</param>
        /// <param name="communication">上下文通讯</param>
        /// <param name="runtimeMode">当前运行模式</param>
        public static void AddRuntimeMode(this IEventBus eventBus, HandlerCommunication communication, string runtimeMode)
        {
            if (communication == null)
                return;

            AddRuntimeMode(eventBus, communication, new Aop.DefaultRuntimeMode() { RuntimeMode = runtimeMode });
        }

        /// <summary>
        /// 新加运行时类型到上下文
        /// </summary>
        /// <param name="eventBus">事件总线</param>
        /// <param name="communication">上下文通讯</param>
        /// <param name="runtimeMode">当前运行模式</param>
        public static void AddRuntimeMode(this IEventBus eventBus, HandlerCommunication communication, Aop.IRuntimeMode runtimeMode)
        {
            if (communication == null || runtimeMode == null)
                return;

            communication.RuntimeModeArray.Add(runtimeMode);
        }

        #endregion
    }
}