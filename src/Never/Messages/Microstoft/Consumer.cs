#if NET461

using System;
using System.Collections.Generic;
using System.Messaging;
using System.Text;

namespace Never.Messages.Microstoft
{
    /// <summary>
    /// MSMQ消费者
    /// </summary>
    public class Consumer : Never.Messages.IMessageConsumer
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
        /// Initializes a new instance of the <see cref="Consumer"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public Consumer(MessageConnection connection)
            : this(connection, new BinaryMessageFormatter())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Consumer"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="formater">The formater.</param>
        /// <exception cref="Never.Exceptions.MessageException">配置MQ路由不能为空</exception>
        public Consumer(MessageConnection connection, IMessageFormatter formater)
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

        #region 同步接受

        /// <summary>
        /// 接收一条消息
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Never.Exceptions.MessageException">获取消息出错</exception>
        public virtual MessagePacket Receive()
        {
            System.Messaging.Message message = null;
            try
            {
                message = this.queue.Receive();
            }
            catch (MessageQueueException)
            {
                throw;
            }

            if (message == null)
                return null;

            return message.Body as MessagePacket;
        }

        /// <summary>
        /// 接收一条消息,但不删除消息
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Never.Exceptions.MessageException">获取消息出错</exception>
        public virtual MessagePacket Peek()
        {
            System.Messaging.Message message = null;
            try
            {
                message = this.queue.Peek();
            }
            catch (MessageQueueException)
            {
                throw;
            }

            if (message == null)
                return null;

            return message.Body as MessagePacket;
        }

        #endregion 同步接受

        #region 异步

        /// <summary>
        /// 异步接收一条消息
        /// </summary>
        /// <param name="messageCallback">回调</param>
        public virtual void ReceiveAsync(Action<MessagePacket> messageCallback)
        {
            this.queue.ReceiveCompleted += new ReceiveCompletedEventHandler((sender, events) =>
            {
                MessageQueue mq = (MessageQueue)sender;
                var message = mq.EndReceive(events.AsyncResult);
                if (messageCallback != null)
                    messageCallback.Invoke(message.Body as MessagePacket);
            });

            this.queue.BeginReceive();
        }

        /// <summary>
        /// 异步接收一条消息,但不删除消息
        /// </summary>
        /// <param name="messageCallback">回调</param>
        public virtual void PeekAsync(Action<MessagePacket> messageCallback)
        {
            this.queue.PeekCompleted += new PeekCompletedEventHandler((sender, events) =>
            {
                MessageQueue mq = (MessageQueue)sender;
                var message = mq.EndPeek(events.AsyncResult);
                if (messageCallback != null)
                    messageCallback.Invoke(message.Body as MessagePacket);
            });

            this.queue.BeginPeek();
        }

        #endregion 异步
    }
}

#endif