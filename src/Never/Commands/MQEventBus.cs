using Never.Events;
using Never.Messages;
using System.Collections.Generic;

namespace Never.Commands
{
    /// <summary>
    /// 使用MQ方式去发布事件
    /// </summary>
    internal sealed class MQEventBus : IEventBus
    {
        #region field

        /// <summary>
        /// 命令总线
        /// </summary>
        private readonly ICommandBus commandBus = null;

        /// <summary>
        /// 消息生产者
        /// </summary>
        private readonly IMessageProducerProvider messageProducerProvider = null;

        /// <summary>
        /// 是否启动了
        /// </summary>
        private bool started = false;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="MQEventBus"/> class.
        /// </summary>
        /// <param name="commandBus"></param>
        /// <param name="messageProducerProvider">The message producer.</param>
        public MQEventBus(ICommandBus commandBus,
            IMessageProducerProvider messageProducerProvider)
        {
            this.commandBus = commandBus;
            this.messageProducerProvider = messageProducerProvider;
        }

        #endregion ctor

        #region push

        public void Push(ICommandContext context, IEnumerable<IEvent[]> events)
        {
            if (events == null)
                return;

            this.Startup();

            var producer = this.messageProducerProvider.MessageProducer;
            foreach (var splits in events)
            {
                if (splits.IsNullOrEmpty())
                    continue;

                foreach (var split in splits)
                    producer.Send(MessagePacket.UseJson(split, this.messageProducerProvider.JsonSerilizer));
            }
        }

        public void Publish<TEvent>(ICommandContext commandContext, TEvent e) where TEvent : IEvent
        {
            if (e == null)
                return;

            this.Startup();

            var producer = this.messageProducerProvider.MessageProducer;
            producer.Send(MessagePacket.UseJson(e, this.messageProducerProvider.JsonSerilizer));
        }

        #endregion push


        /// <summary>
        ///
        /// </summary>
        public void Startup()
        {
            if (this.messageProducerProvider.MessageProducer != null && !this.started)
            {
                this.started = true;
                this.messageProducerProvider.MessageProducer.Startup();
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void Shutdown()
        {
            if (this.messageProducerProvider.MessageProducer != null && this.started)
            {
                this.started = false;
                this.messageProducerProvider.MessageProducer.Shutdown();
            }
        }
    }
}