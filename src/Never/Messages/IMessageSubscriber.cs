namespace Never.Messages
{
    /// <summary>
    /// 订阅者
    /// </summary>
    public interface IMessageSubscriber
    {
    }

    /// <summary>
    /// 订阅者
    /// </summary>
    /// <typeparam name="TMessage">消息类型</typeparam>
    public interface IMessageSubscriber<in TMessage> : IMessageSubscriber
        where TMessage : IMessage
    {
        /// <summary>
        /// 消息处理约定
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="message">消息消息</param>
        void Execute(IMessageContext context, TMessage message);
    }
}