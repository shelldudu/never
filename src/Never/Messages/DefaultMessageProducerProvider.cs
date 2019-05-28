using Never.Serialization;

namespace Never.Messages
{
    /// <summary>
    /// 根据事件类型去查询消息提供者
    /// </summary>
    public class DefaultMessageProducerProvider : IMessageProducerProvider
    {
        #region field

        /// <summary>
        ///
        /// </summary>
        private readonly IMessageProducer messageProducer = null;

        /// <summary>
        ///
        /// </summary>
        private readonly IJsonSerializer jsonSerializer = null;

        #endregion field

        #region IMessageProducerProvider

        /// <summary>
        /// 查询Json序列化接口
        /// </summary>
        /// <returns></returns>
        public IJsonSerializer JsonSerilizer { get { return this.jsonSerializer; } }

        /// <summary>
        /// 查询消息提供者
        /// </summary>
        /// <returns></returns>
        public IMessageProducer MessageProducer { get { return this.messageProducer; } }

        #endregion IMessageProducerProvider

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMessageProducerProvider"/> class.
        /// </summary>
        public DefaultMessageProducerProvider()
            : this(EmptyMessageProducer.Empty, Never.Serialization.SerializeEnvironment.JsonSerializer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMessageProducerProvider"/> class.
        /// </summary>
        /// <param name="messageProducer">The message producer.</param>
        public DefaultMessageProducerProvider(IMessageProducer messageProducer)
            : this(messageProducer, Never.Serialization.SerializeEnvironment.JsonSerializer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMessageProducerProvider"/> class.
        /// </summary>
        /// <param name="jsonSerializer">The json serializer.</param>
        /// <param name="messageProducer">The message producer.</param>
        public DefaultMessageProducerProvider(IMessageProducer messageProducer, IJsonSerializer jsonSerializer)
        {
            this.messageProducer = messageProducer ?? EmptyMessageProducer.Empty;
            this.jsonSerializer = jsonSerializer ?? Never.Serialization.SerializeEnvironment.JsonSerializer;
        }

        #endregion ctor
    }
}