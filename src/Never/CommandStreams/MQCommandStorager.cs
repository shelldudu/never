using Never.Aop;
using Never.Aop.DomainFilters;
using Never.Commands;
using Never.Logging;
using Never.Messages;
using Never.Serialization;
using System;

namespace Never.CommandStreams
{
    /// <summary>
    /// 使用MQ消息去保存命令信息
    /// 该命令会被包装为<see cref="CommandStreamMessage"/> 并发送到指定队列中
    /// </summary>
    public class MQCommandStorager : ICommandStorager, IWorkService
    {
        #region field

        /// <summary>
        /// 消息生产者
        /// </summary>
        private readonly IMessageProducer messageProducer = null;

        /// <summary>
        /// 日志接口
        /// </summary>
        private readonly ILoggerBuilder loggerBuilder = null;

        /// <summary>
        /// Json序列器
        /// </summary>
        private readonly IJsonSerializer jsonSerializer = null;

        /// <summary>
        /// 是否启动了
        /// </summary>
        private bool started = false;
        #endregion field

        #region prop

        /// <summary>
        /// 消息生产者
        /// </summary>
        public IMessageProducer MessageProducer
        {
            get
            {
                return this.messageProducer;
            }
        }

        /// <summary>
        /// Json序列器
        /// </summary>
        public IJsonSerializer JsonSerializer
        {
            get
            {
                return this.jsonSerializer;
            }
        }

        #endregion prop

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="MQCommandStorager"/> class.
        /// </summary>
        /// <param name="messageProducer">The message producer.</param>
        public MQCommandStorager(IMessageProducer messageProducer)
            : this(messageProducer, SerializeEnvironment.JsonSerializer, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MQCommandStorager"/> class.
        /// </summary>
        /// <param name="messageProducer">The message producer.</param>
        /// <param name="jsonSerializer">The json serializer.</param>
        public MQCommandStorager(IMessageProducer messageProducer, IJsonSerializer jsonSerializer)
             : this(messageProducer, jsonSerializer, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MQCommandStorager"/> class.
        /// </summary>
        /// <param name="messageProducerProvider">The message producer.</param>
        public MQCommandStorager(IMessageProducerProvider messageProducerProvider)
            : this(messageProducerProvider.MessageProducer, SerializeEnvironment.JsonSerializer, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MQCommandStorager"/> class.
        /// </summary>
        /// <param name="messageProducer">The message producer.</param>
        /// <param name="jsonSerializer">The json serializer.</param>
        /// <param name="loggerBuilder">The logger builder.</param>
        protected MQCommandStorager(IMessageProducer messageProducer, IJsonSerializer jsonSerializer, ILoggerBuilder loggerBuilder)
        {
            this.messageProducer = messageProducer;
            this.jsonSerializer = jsonSerializer ?? SerializeEnvironment.JsonSerializer;
            this.loggerBuilder = loggerBuilder ?? LoggerBuilder.Empty;
        }

        #endregion ctor

        /// <summary>
        /// 处理异常
        /// </summary>
        /// <param name="ex"></param>
        public virtual void HandlerException(Exception ex)
        {
            this.loggerBuilder.Build(typeof(MQCommandStorager)).Error("批量保存命令失败", ex);
        }

        /// <summary>
        /// 分析
        /// </summary>
        /// <param name="commandContext"></param>
        /// <param name="command"></param>
        public void Save<T>(ICommandContext commandContext, T command) where T : ICommand
        {
            if (this.CommandFilter(command, CommandBehaviorStorager.Default))
                this.Send(command);
        }

        /// <summary>
        /// 发送一个命令，该命令会被包装为<see cref="CommandStreamMessage"/> 并发送到指定队列中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">The command</param>
        protected virtual void Send<T>(T command) where T : ICommand
        {
            if (this.messageProducer == null)
                return;

            this.Startup();

            try
            {
                this.messageProducer.Send(new MessagePacket()
                {
                    ContentType = MessagePacket.GetContentType(typeof(T)),
                    Body = this.jsonSerializer.Serialize(command),
                }, null);
            }
            catch (Exception ex)
            {
                this.HandlerException(ex);
            }
        }

        /// <summary>
        /// 命令过滤器，如果为true,则发送命令，否则不发送
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">The command</param>
        /// <param name="commandBehaviorStorage">The Command behavior storage.</param>
        /// <returns></returns>
        protected virtual bool CommandFilter<T>(T command, CommandBehaviorStorager commandBehaviorStorage) where T : ICommand
        {
            return true;
        }

        /// <summary>
        ///
        /// </summary>
        public void Startup()
        {
            if (this.messageProducer != null && !this.started)
            {
                this.started = true;
                this.messageProducer.Startup();
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void Shutdown()
        {
            if (this.messageProducer != null && this.started)
            {
                this.started = false;
                this.messageProducer.Shutdown();
            }
        }
    }
}