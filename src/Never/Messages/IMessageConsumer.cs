using System;

namespace Never.Messages
{
    /// <summary>
    /// 消息消费者
    /// </summary>
    public interface IMessageConsumer : IWorkService
    {
        /// <summary>
        /// 消息路由
        /// </summary>
        IMessageConnection MessageConnection { get; }

        /// <summary>
        /// 接收一条消息
        /// </summary>
        /// <returns></returns>
        MessagePacket Receive();

        /// <summary>
        /// 异步接收一条消息
        /// </summary>
        /// <param name="messageCallback">回调</param>
        void ReceiveAsync(Action<MessagePacket> messageCallback);
    }
}