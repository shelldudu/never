using System.Threading.Tasks;

namespace Never.Messages
{
    /// <summary>
    /// 发布者
    /// </summary>
    public interface IMessagePublisher
    {
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="TMessage">消息类型</typeparam>
        /// <param name="context">上下文通讯</param>
        /// <param name="message">消息</param>
        void Publish<TMessage>(IMessageContext context, TMessage message)
            where TMessage : IMessage;
    }
}