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
    public class MessageDispatcher : Never.Threading.ThreadCircler, IWorkService
    {
        #region field

        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILoggerBuilder loggerBuilder = null;

        /// <summary>
        /// 消息消费者
        /// </summary>
        private readonly IMessageConsumer messageConsumer = null;

        /// <summary>
        /// json序列号接口
        /// </summary>
        private readonly IJsonSerializer jsonSerializer = null;

        /// <summary>
        /// 睡眠时间
        /// </summary>
        private readonly TimeSpan sleepTimeSpan = TimeSpan.Zero;
        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageDispatcher"/> class.
        /// </summary>
        /// <param name="messageConsumer">消费者接口</param>
        /// <param name="jsonSerializer">json序列号接口</param>
        public MessageDispatcher(IMessageConsumer messageConsumer, IJsonSerializer jsonSerializer)
            : this(messageConsumer, jsonSerializer, LoggerBuilder.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageDispatcher"/> class.
        /// </summary>
        /// <param name="messageConsumer">消费者接口</param>
        /// <param name="jsonSerializer">json序列号接口</param>
        /// <param name="loggerBuilder">统计消息发布日志，因为不同的发布者可能都有自己的日志发布，所以这个接口通常不用赋值</param>
        public MessageDispatcher(IMessageConsumer messageConsumer, IJsonSerializer jsonSerializer, ILoggerBuilder loggerBuilder)
            : this(messageConsumer, jsonSerializer, loggerBuilder, TimeSpan.Zero)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageDispatcher"/> class.
        /// </summary>
        /// <param name="messageConsumer">消费者接口</param>
        /// <param name="jsonSerializer">json序列号接口</param>
        /// <param name="sleepTimeSpan">用于控制线程while的频率</param>
        /// <param name="loggerBuilder">统计消息发布日志，因为不同的发布者可能都有自己的日志发布，所以这个接口通常不用赋值</param>
        public MessageDispatcher(IMessageConsumer messageConsumer, IJsonSerializer jsonSerializer, ILoggerBuilder loggerBuilder, TimeSpan sleepTimeSpan) : base(null, typeof(MessageDispatcher).Name)
        {
            if (messageConsumer == null)
                throw new ArgumentNullException("消费者接口不能为空");

            if (jsonSerializer == null)
                throw new ArgumentNullException("json序列号接口不能为空");

            this.messageConsumer = messageConsumer;
            this.jsonSerializer = jsonSerializer;
            this.loggerBuilder = loggerBuilder ?? LoggerBuilder.Empty;
            this.sleepTimeSpan = sleepTimeSpan;

            this.Replace(Change);
        }

        #endregion ctor

        #region prop

        /// <summary>
        /// 在接收到数据的时候
        /// </summary>
        public event EventHandler<MessageDispathEvargs> OnReceiving;

        /// <summary>
        /// 消息事件数据
        /// </summary>
        public class MessageDispathEvargs : EventArgs
        {
            /// <summary>
            /// 消息
            /// </summary>
            public MessagePacket Message { get; set; }
        }

        /// <summary>
        /// 消息发布者
        /// </summary>
        public IMessagePublisher Publisher { get; set; }

        /// <summary>
        /// 事件总线
        /// </summary>
        public IEventBus EventBus { get; set; }

        /// <summary>
        /// 命令总线
        /// </summary>
        public ICommandBus CommandBus { get; set; }

        #endregion

        #region AbstThreadCircling

        /// <summary>
        /// 当线程方法出错的时候，子类该如何处理日志
        /// </summary>
        /// <param name="ex">异常信息</param>
        /// <param name="message">异常消息</param>
        protected override void HandleException(Exception ex, string message)
        {
            this.loggerBuilder.Build(typeof(MessageDispatcher)).Error(message, ex);
        }

        /// <summary>
        /// 需要重复执行的任务状态，true表示完成，则线程会用WaitTime休眠，否则false则表示未完成，线程会用DozeTime休眠
        /// </summary>
        /// <returns></returns>
        protected TimeSpan Change()
        {
            /*注意，这里可能是会阻塞的，所以我们要发一条提醒短信，间隔为1分钟发一条 */
            var packet = this.messageConsumer.Receive();
            if (packet == null)
                return this.sleepTimeSpan;

            this.OnReceiving?.Invoke(this, new MessageDispathEvargs() { Message = packet });
            if (string.IsNullOrEmpty(packet.ContentType))
                return this.sleepTimeSpan;

            var type = Type.GetType(packet.ContentType);
            if (type == null)
                return this.sleepTimeSpan;

            var @object = this.DeserializeObject(packet);
            if (this.Publish(this.Publisher, @object as IMessage))
                return this.sleepTimeSpan;

            if (this.Publish(this.CommandBus, @object as ICommand))
                return this.sleepTimeSpan;

            if (this.Publish(this.EventBus, @object as IEvent))
                return this.sleepTimeSpan;

            /*请一定要返回false(表示不打瞌睡，不停工)，因为没有外部线程去唤醒自身，自身线程会睡眠而不会工作*/
            return this.sleepTimeSpan;
        }

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        protected virtual object DeserializeObject(MessagePacket packet)
        {
            var type = Type.GetType(packet.ContentType);
            if (type == null)
                return null;

            return this.jsonSerializer.DeserializeObject(packet.Body, type);
        }

        /// <summary>
        /// 发布消息，处理成功完后返回true，其他返回false
        /// </summary>
        /// <param name="publisher"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected virtual bool Publish(IMessagePublisher publisher, IMessage message)
        {
            if (publisher == null || message == null)
                return false;

            publisher.PublishMessage(message);
            return true;
        }

        /// <summary>
        /// 发布命令，处理成功完后返回true，其他返回false
        /// </summary>
        /// <param name="publisher"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected virtual bool Publish(ICommandBus publisher, ICommand message)
        {
            if (publisher == null || message == null)
                return false;

            publisher.SendCommand(message, new HandlerCommunication());
            return true;
        }

        /// <summary>
        /// 发布时间，处理成功完后返回true，其他返回false
        /// </summary>
        /// <param name="publisher"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected virtual bool Publish(IEventBus publisher, IEvent message)
        {
            if (publisher == null || message == null)
                return false;

            publisher.PublishEvent(message);
            return true;
        }

        #endregion AbstThreadCircling

        #region start and stop

        /// <summary>
        /// 工作开始了
        /// </summary>
        protected override void OnStarting()
        {
            if (this.IsWorking)
                return;

            this.messageConsumer.Startup();
            base.OnStarting();
        }

        /// <summary>
        /// 工作停止了
        /// </summary>
        protected override void OnClosed()
        {
            if (!this.IsWorking)
                return;

            this.messageConsumer.Shutdown();
            base.OnClosed();
        }

        /// <summary>
        /// 工作开始了
        /// </summary>
        public void Startup()
        {
            this.Start();
        }

        /// <summary>
        /// 工作停止了
        /// </summary>
        public void Shutdown()
        {
            this.Close();
        }

        #endregion start and stop
    }
}