using Never.Logging;
using System;

namespace Never.Messages
{
    /// <summary>
    /// 消息执行过程中一些元素
    /// </summary>
    public struct MessageExcutingElement
    {
        /// <summary>
        /// 日志
        /// </summary>
        public ILoggerBuilder LoggerBuilder { get; set; }

        /// <summary>
        /// 消息处理者
        /// </summary>
        public IMessageSubscriber MessageHandler { get; set; }

        /// <summary>
        /// 消息类型，通常只有一个
        /// </summary>
        public Type MessageHandlerType { get; set; }

        /// <summary>
        /// 消息上下文
        /// </summary>
        public IMessageContext MessageContext { get; set; }
    }
}