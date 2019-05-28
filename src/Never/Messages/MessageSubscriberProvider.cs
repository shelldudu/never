using Never.IoC;

namespace Never.Messages
{
    /// <summary>
    /// 消息提供者
    /// </summary>
    public class MessageSubscriberProvider
    {
        #region field

        /// <summary>
        /// 服务定位者
        /// </summary>
        private readonly ILifetimeScope scope = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageSubscriberProvider"/> class.
        /// </summary>
        public MessageSubscriberProvider(ILifetimeScope scope)
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

        #region IMessageProvider成员

        /// <summary>
        /// 查找所有监听了T类型的消息监听者
        /// </summary>
        /// <typeparam name="TMessage">消息类型</typeparam>
        /// <returns></returns>
        public IMessageSubscriber<TMessage>[] FindSubscriber<TMessage>() where TMessage : IMessage
        {
            return this.scope.ResolveAll<IMessageSubscriber<TMessage>>();
        }

        /// <summary>
        /// 获取消息上下文提供者
        /// </summary>
        /// <returns></returns>
        public IMessageContext FindMessageContext()
        {
            return this.scope.ResolveOptional<IMessageContext>() ?? new DefaultMessageContext();
        }

        #endregion IMessageProvider成员
    }
}