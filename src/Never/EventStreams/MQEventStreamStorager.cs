using Never.Aop;
using Never.Aop.DomainFilters;
using Never.Commands;
using Never.Events;
using Never.Logging;
using Never.Messages;
using Never.Serialization;
using System;
using System.Collections.Generic;

namespace Never.EventStreams
{
    /// <summary>
    /// 使用MQ消息去保存事件信息
    /// 该事件会被包装为<see cref="EventStreamMessage"/> 并发送到指定队列中
    /// </summary>
    public class MQEventStreamStorager : IEventStorager, IEventStreamStorager, IWorkService
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
        /// 过滤器
        /// </summary>
        private readonly Func<IOperateEvent, EventBehaviorStorager, bool> eventFilter = null;

        /// <summary>
        /// 事件分析者
        /// </summary>
        private readonly IEventStreamAnalyser eventStreamAnalyser = null;

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
        /// Initializes a new instance of the <see cref="MQEventStreamStorager"/> class.
        /// </summary>
        /// <param name="messageProducer">The message producer.</param>
        public MQEventStreamStorager(IMessageProducer messageProducer)
            : this(messageProducer, SerializeEnvironment.JsonSerializer, null, LoggerBuilder.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MQEventStreamStorager"/> class.
        /// </summary>
        /// <param name="messageProducer">The message producer.</param>
        /// <param name="jsonSerializer">The json serializer.</param>
        public MQEventStreamStorager(IMessageProducer messageProducer, IJsonSerializer jsonSerializer)
             : this(messageProducer, jsonSerializer, null, LoggerBuilder.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MQEventStreamStorager"/> class.
        /// </summary>
        /// <param name="messageProducer">The message producer.</param>
        /// <param name="eventFilter">The eventFilter.</param>
        public MQEventStreamStorager(IMessageProducer messageProducer, Func<IOperateEvent, EventBehaviorStorager, bool> eventFilter)
            : this(messageProducer, SerializeEnvironment.JsonSerializer, null, LoggerBuilder.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MQEventStreamStorager"/> class.
        /// </summary>
        /// <param name="messageProducer">The message producer.</param>
        /// <param name="jsonSerializer">The json serializer.</param>
        /// <param name="eventFilter">The eventFilter.</param>
        public MQEventStreamStorager(IMessageProducer messageProducer, IJsonSerializer jsonSerializer, Func<IOperateEvent, EventBehaviorStorager, bool> eventFilter)
            : this(messageProducer, jsonSerializer, null, LoggerBuilder.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MQEventStreamStorager"/> class.
        /// </summary>
        /// <param name="messageProducer">The message producer.</param>
        /// <param name="jsonSerializer">The json serializer.</param>
        /// <param name="loggerBuilder">The logger builder.</param>
        protected MQEventStreamStorager(IMessageProducer messageProducer, IJsonSerializer jsonSerializer, ILoggerBuilder loggerBuilder)
               : this(messageProducer, jsonSerializer, null, loggerBuilder)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MQEventStreamStorager"/> class.
        /// </summary>
        /// <param name="messageProducer">The message producer.</param>
        /// <param name="jsonSerializer">The json serializer.</param>
        /// <param name="eventFilter">The eventFilter.</param>
        /// <param name="loggerBuilder">The logger builder.</param>
        protected MQEventStreamStorager(IMessageProducer messageProducer, IJsonSerializer jsonSerializer, Func<IOperateEvent, EventBehaviorStorager, bool> eventFilter, ILoggerBuilder loggerBuilder)
        {
            this.messageProducer = messageProducer;
            this.jsonSerializer = jsonSerializer ?? SerializeEnvironment.JsonSerializer;
            this.loggerBuilder = loggerBuilder ?? LoggerBuilder.Empty;
            this.eventFilter = eventFilter;
            this.eventStreamAnalyser = new DefaultEventStreamAnalyser();
        }

        #endregion ctor

        /// <summary>
        /// 处理异常
        /// </summary>
        /// <param name="ex"></param>
        public virtual void HandlerException(Exception ex)
        {
            this.loggerBuilder.Build(typeof(MQEventStreamStorager)).Error("批量保存事件失败", ex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandContext"></param>
        /// <param name="events"></param>
        public void Save(ICommandContext commandContext, IEnumerable<KeyValuePair<Type, IEvent[]>> events)
        {
            this.Save(this.eventStreamAnalyser.Analyse(commandContext, events));
        }

        /// <summary>
        /// 批量保存领域事件
        /// </summary>
        /// <param name="events">事件列表</param>
        /// <returns></returns>
        public void Save(IEnumerable<IOperateEvent> events)
        {
            if (events == null)
                return;

            foreach (var e in events)
            {
                if (this.EventFilter(e, EventBehaviorStorager.Default))
                    this.Send(e);
            }
        }

        /// <summary>
        /// 发送一个事件，该事件会被包装为<see cref="EventStreamMessage"/> 并发送到指定队列中
        /// </summary>
        /// <param name="e">The e.</param>
        protected virtual void Send(IOperateEvent e)
        {
            if (this.messageProducer == null)
                return;

            this.Startup();

            var message = e.ConvertTo(this.jsonSerializer);

            try
            {
                this.messageProducer.Send(new MessagePacket()
                {
                    ContentType = MessagePacket.GetContentType(message),
                    Body = this.jsonSerializer.Serialize(message),
                }, null);
            }
            catch (Exception ex)
            {
                this.HandlerException(ex);
            }
        }

        /// <summary>
        /// 事件过滤器，如果为true,则发送事件，否则不发送
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="eventBehaviorStorager">The event behavior storage.</param>
        /// <returns></returns>
        protected virtual bool EventFilter(IOperateEvent e, EventBehaviorStorager eventBehaviorStorager)
        {
            if (this.eventFilter != null)
                return this.eventFilter.Invoke(e, eventBehaviorStorager);

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