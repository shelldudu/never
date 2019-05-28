namespace Never.Messages
{
    /// <summary>
    /// 消息生产者接口
    /// </summary>
    public interface IMessageProducer : IWorkService
    {
        /// <summary>
        /// 消息路由
        /// </summary>
        IMessageConnection MessageConnection { get; }

        /// <summary>
        /// 发送一条消息
        /// </summary>
        /// <param name="message">消息</param>
        void Send(MessagePacket message);

        /// <summary>
        /// 发送一条消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="route">消息路由</param>
        void Send(MessagePacket message, IMessageRoute route);
    }
}