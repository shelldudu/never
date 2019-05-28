using Never.Commands;
using Never.Events;
using Never.Logging;
using Never.Serialization;
using System;

namespace Never.Messages
{
    /// <summary>
    /// 消息调度，构建后并不会立即启动
    /// </summary>
    public class MessageTaskDispatcher : MessageDispatcher, IWorkService
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageTaskDispatcher"/> class.
        /// </summary>
        /// <param name="messageConsumer">消费者接口</param>
        /// <param name="jsonSerializer">json序列号接口</param>
        public MessageTaskDispatcher(IMessageConsumer messageConsumer, IJsonSerializer jsonSerializer)
            : base(messageConsumer, jsonSerializer, LoggerBuilder.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageTaskDispatcher"/> class.
        /// </summary>
        /// <param name="messageConsumer">消费者接口</param>
        /// <param name="jsonSerializer">json序列号接口</param>
        /// <param name="loggerBuilder">统计消息发布日志，因为不同的发布者可能都有自己的日志发布，所以这个接口通常不用赋值</param>
        public MessageTaskDispatcher(IMessageConsumer messageConsumer, IJsonSerializer jsonSerializer, ILoggerBuilder loggerBuilder)
            : base(messageConsumer, jsonSerializer, loggerBuilder, TimeSpan.Zero)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageTaskDispatcher"/> class.
        /// </summary>
        /// <param name="messageConsumer">消费者接口</param>
        /// <param name="jsonSerializer">json序列号接口</param>
        /// <param name="sleepTimeSpan">用于控制线程whild的频率</param>
        /// <param name="loggerBuilder">统计消息发布日志，因为不同的发布者可能都有自己的日志发布，所以这个接口通常不用赋值</param>
        public MessageTaskDispatcher(IMessageConsumer messageConsumer, IJsonSerializer jsonSerializer, ILoggerBuilder loggerBuilder, TimeSpan sleepTimeSpan)
             : base(messageConsumer, jsonSerializer, loggerBuilder, sleepTimeSpan)
        {
        }

        #endregion ctor

        #region AbstThreadCircling

        /// <summary>
        /// 发布消息，处理成功完后返回true，其他返回false
        /// </summary>
        /// <param name="publisher"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected override bool Publish(IMessagePublisher publisher, IMessage message)
        {
            if (publisher == null || message == null)
                return false;

            publisher.PublishMessageAsync( message);
            return true;
        }

        /// <summary>
        /// 发布命令，处理成功完后返回true，其他返回false
        /// </summary>
        /// <param name="publisher"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected override bool Publish(ICommandBus publisher, ICommand message)
        {
            if (publisher == null || message == null)
                return false;

            publisher.SendCommandAsync(message, new HandlerCommunication());
            return true;
        }

        /// <summary>
        /// 发布时间，处理成功完后返回true，其他返回false
        /// </summary>
        /// <param name="publisher"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected override bool Publish(IEventBus publisher, IEvent message)
        {
            if (publisher == null || message == null)
                return false;

            publisher.PublishEvent(message);
            return true;
        }

        #endregion AbstThreadCircling
    }
}