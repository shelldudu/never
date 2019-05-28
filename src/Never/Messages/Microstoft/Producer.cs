#if NET461

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Messaging;
using System.Text;

namespace Never.Messages.Microstoft
{
    /// <summary>
    /// 生产者
    /// </summary>
    public class Producer : IMessageProducer
    {
        #region field

        /// <summary>
        /// mq队列
        /// </summary>
        private readonly MessageQueue queue = null;

        /// <summary>
        /// 是否启动
        /// </summary>
        private readonly bool isStart = false;

        /// <summary>
        /// 是否关闭
        /// </summary>
        private bool isShutDown = false;

        /// <summary>
        /// connection
        /// </summary>
        private readonly MessageConnection connection = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="Producer"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public Producer(MessageConnection connection)
            : this(connection, new BinaryMessageFormatter())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Producer"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="formater">The formater.</param>
        /// <exception cref="Never.Exceptions.MessageException">配置MQ路由不能为空</exception>
        public Producer(MessageConnection connection, IMessageFormatter formater)
        {
            this.connection = connection;
            if (connection == null || string.IsNullOrEmpty(connection.ConnetctionString))
                throw new ArgumentNullException("配置MQ路由不能为空");

            this.queue = new MessageQueue()
            {
                Formatter = formater ?? new BinaryMessageFormatter(),
                Path = connection.ConnetctionString
            };
            this.queue.MessageReadPropertyFilter.Priority = true;
        }

        #endregion ctor

        #region 路由

        /// <summary>
        /// 消息路由
        /// </summary>
        public virtual Never.Messages.IMessageConnection MessageConnection
        {
            get
            {
                return this.connection;
            }
        }

        #endregion 路由

        #region 启动与关闭

        /// <summary>
        /// 启动
        /// </summary>
        public virtual void Startup()
        {
            if (this.isStart)
                return;

            if (!this.queue.CanWrite)
                throw new ArgumentException(string.Format("当前队列{0}不可写", this.queue.Path));
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public virtual void Shutdown()
        {
            if (this.isShutDown)
                return;

            this.queue.Close();
            this.isShutDown = true;
        }

        #endregion 启动与关闭

        #region 发送

        /// <summary>
        /// 发送一条消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <exception cref="Never.Exceptions.MessageException">发送消息出错</exception>
        public virtual void Send(MessagePacket message)
        {
            this.Send(message, null);
        }

        /// <summary>
        /// 发送一条消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="route">消息路由</param>
        /// <exception cref="Never.Exceptions.MessageException">发送消息出错</exception>
        public virtual void Send(MessagePacket message, IMessageRoute route)
        {
            var @object = new System.Messaging.Message(message, this.queue.Formatter) { Priority = System.Messaging.MessagePriority.Normal };
            try
            {
                this.queue.Send(@object, MessageQueueTransactionType.None);
            }
            catch (MessageQueueException)
            {
                throw;
            }
        }

        #endregion 发送
    }
}

#endif