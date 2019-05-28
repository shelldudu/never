using Never.Exceptions;
using Never.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Never.Messages
{
    /// <summary>
    /// 消息总线
    /// </summary>
    public class MessagePublisher : IMessagePublisher
    {
        #region field

        /// <summary>
        /// 消息提供者
        /// </summary>
        protected readonly IServiceLocator serviceLocator = null;

        /// <summary>
        /// 消息对象
        /// </summary>
        protected readonly static IMessage messageObject = default(IMessage);

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagePublisher"/> class.
        /// </summary>
        /// <param name="serviceLocator">消息提供者.</param>
        public MessagePublisher(IServiceLocator serviceLocator)
        {
            this.serviceLocator = serviceLocator;
        }

        #endregion ctor

        #region TMessage1

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="TMessage">信息类型</typeparam>
        /// <param name="messageContext">上下文通讯</param>
        /// <param name="message">信息</param>
        public void Publish<TMessage>(IMessageContext messageContext, TMessage message)
             where TMessage : IMessage
        {
            if (messageContext == null)
                messageContext = new DefaultMessageContext();

            var elements = this.FindMessageExcutingElement(messageContext, message);
            if (elements == null || elements.Length <= 0)
                return;

            foreach (var element in elements)
            {
                /*默认的消息上下文*/
                var defaulTMessageContext = element.MessageContext as DefaultMessageContext;
                if (defaulTMessageContext != null)
                    defaulTMessageContext.TargetType = element.MessageHandlerType;
                if (element.LoggerBuilder != null)
                    element.MessageContext.Items["LoggerBuilder"] = element.LoggerBuilder;
            }

            foreach (var element in elements)
            {
                this.CallMessageInvoke(message, element);
            }

            this.Release(messageContext);
        }


        #endregion TMessage1

        #region utils

        /// <summary>
        /// 查询执行元素
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="message"></param>
        /// <param name="messageContext"></param>
        /// <returns></returns>
        private MessageExcutingElement[] FindMessageExcutingElement<TMessage>(IMessageContext messageContext, TMessage message)
            where TMessage : IMessage
        {
            try
            {
                var provider = new MessageSubscriberProvider(this.serviceLocator.BeginLifetimeScope());
                var elements = MessageExcutingHelper.FindMessageExcutingElement(provider, message);
                messageContext.Items["BeginLifetimeScope"] = provider.Scope;
                return elements;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="messageContext"></param>
        protected void Release(IMessageContext messageContext)
        {
            var scope = messageContext.Items != null && messageContext.Items.ContainsKey("BeginLifetimeScope") ? messageContext.Items["BeginLifetimeScope"] as ILifetimeScope : null;
            if (scope != null)
            {
                try
                {
                    scope.Dispose();
                    messageContext.Items.Remove("BeginLifetimeScope");
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// 生成方法
        /// </summary>
        /// <typeparam name="TMessage">消息类型</typeparam>
        /// <param name="e">消息</param>
        /// <param name="element">执行元素</param>
        protected void CallMessageInvoke<TMessage>(TMessage e, MessageExcutingElement element) where TMessage : IMessage
        {
            try
            {
                ((IMessageSubscriber<TMessage>)element.MessageHandler).Execute(element.MessageContext, e);
            }
            catch (Exception ex)
            {
                if (element.LoggerBuilder != null)
                    this.OnMessageError(element, e, ex);
            }
        }

        /// <summary>
        /// 保存日志
        /// </summary>
        /// <typeparam name="TMessage">消息类型</typeparam>
        /// <param name="element">执行元素</param>
        /// <param name="e">消息</param>
        /// <param name="ex">异常</param>
        protected void OnMessageError<TMessage>(MessageExcutingElement element, TMessage e, Exception ex) where TMessage : IMessage
        {
            try
            {
                element.LoggerBuilder.Build(element.MessageHandlerType).Error(string.Format("处理消息出错MessageType={0},ex:", typeof(TMessage).FullName), ex);
            }
            catch
            {
            }
        }

        #endregion utils
    }
}