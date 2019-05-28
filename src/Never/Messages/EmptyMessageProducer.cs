namespace Never.Messages
{
    /// <summary>
    /// 空的消息发送者
    /// </summary>
    public class EmptyMessageProducer : IMessageProducer
    {
        #region ctor

        /// <summary>
        ///
        /// </summary>
        public EmptyMessageProducer()
        {
        }

        #endregion ctor

        #region IMessageProducer

        /// <summary>
        /// 消息路由
        /// </summary>
        public IMessageConnection MessageConnection
        {
            get
            {
                return default(IMessageConnection);
            }
        }

        /// <summary>
        /// 发送一条消息
        /// </summary>
        /// <param name="message">消息</param>
        public void Send(MessagePacket message)
        {
        }

        /// <summary>
        /// 发送一条消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="route"></param>
        public void Send(MessagePacket message, IMessageRoute route)
        {
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Shutdown()
        {
        }

        /// <summary>
        /// 启动
        /// </summary>
        public void Startup()
        {
        }

        #endregion IMessageProducer

        #region only

        /// <summary>
        /// 空单例
        /// </summary>
        public static EmptyMessageProducer Empty
        {
            get
            {
                if (Singleton<EmptyMessageProducer>.Instance == null)
                    Singleton<EmptyMessageProducer>.Instance = new EmptyMessageProducer();

                return Singleton<EmptyMessageProducer>.Instance;
            }
        }

        #endregion only
    }
}