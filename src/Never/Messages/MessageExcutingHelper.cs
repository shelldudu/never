using Never.Aop;
using Never.Aop.DomainFilters;
using Never.IoC;
using Never.Logging;
using System.Collections.Generic;

namespace Never.Messages
{
    /// <summary>
    /// 消息扩展
    /// </summary>
    public static class MessageExcutingHelper
    {
        #region element

        internal static MessageExcutingElement[] FindMessageExcutingElement<TMessage>(MessageSubscriberProvider provider, TMessage e) where TMessage : IMessage
        {
            var messageHandlers = provider.FindSubscriber<TMessage>();
            if (messageHandlers == null || messageHandlers.Length == 0)
                return new MessageExcutingElement[] { };

            /*先获取到每个MessageHandler的所有属性*/
            var elements = new List<MessageExcutingElement>(messageHandlers.Length);
            for (var i = 0; i < messageHandlers.Length; i++)
            {
                var item = messageHandlers[i];
                var handlerType = item.GetType();
                var attributes = HandlerBehaviorStorager.Default.GetAttributes(handlerType);
                var loggerAttribute = ObjectExtension.GetAttribute<LoggerAttribute>(attributes);
                ILoggerBuilder loggerBuilder = null;
                if (loggerAttribute != null)
                {
                    try
                    {
                        loggerBuilder = provider.Scope.Resolve<ILoggerBuilder>(loggerAttribute.RegisterKey);
                    }
                    catch
                    {
                    }
                }

                elements.Add(new MessageExcutingElement()
                {
                    MessageContext = provider.FindMessageContext(),
                    MessageHandler = item,
                    MessageHandlerType = handlerType,
                    LoggerBuilder = loggerBuilder,
                });
            }

            return elements.ToArray();
        }

        #endregion
    }
}