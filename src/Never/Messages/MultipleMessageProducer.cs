using System;
using System.Collections.Generic;

namespace Never.Messages
{
    /// <summary>
    /// 多个消息发送者
    /// </summary>
    public class MultipleMessageProducer : IMessageProducer, IWorkService
    {
        #region field

        /// <summary>
        ///  事件流接口集合
        /// </summary>
        private IEnumerable<IMessageProducer> storager = null;

        /// <summary>
        ///  创建事件流接口集合
        /// </summary>
        private readonly Func<IEnumerable<IMessageProducer>> callback = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleMessageProducer"/> class.
        /// </summary>
        /// <param name="storager">The storages.</param>
        public MultipleMessageProducer(IEnumerable<IMessageProducer> storager)
        {
            this.storager = storager;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleMessageProducer"/> class.
        /// </summary>
        /// <param name="callback">The callback.</param>
        public MultipleMessageProducer(Func<IEnumerable<IMessageProducer>> callback)
        {
            this.callback = callback;
        }

        #endregion ctor

        #region storager

        /// <summary>
        /// 查询接口
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IMessageProducer> FindStorager()
        {
            if (this.storager != null)
                return this.storager;

            if (this.callback == null)
                return null;

            lock (this)
            {
                this.storager = this.callback.Invoke();
                if (this.storager == null)
                    return new IMessageProducer[] { };

                return this.storager;
            }
        }

        #endregion storager

        #region IMessageProducer

        /// <summary>
        /// 消息路由
        /// </summary>
        public IMessageConnection MessageConnection
        {
            get
            {
                foreach (var i in this.FindStorager())
                {
                    return i.MessageConnection;
                }

                return null;
            }
        }

        /// <summary>
        /// 发送一条消息
        /// </summary>
        /// <param name="message">消息</param>
        public void Send(MessagePacket message)
        {
            this.Send(message, null);
        }

        /// <summary>
        /// 发送一条消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="route">消息路由</param>
        public void Send(MessagePacket message, IMessageRoute route)
        {
            foreach (var i in this.FindStorager())
            {
                i.Send(message, route);
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Shutdown()
        {
            foreach (var i in this.FindStorager())
            {
                i.Shutdown();
            }
        }

        /// <summary>
        /// 启动
        /// </summary>
        public void Startup()
        {
            foreach (var i in this.FindStorager())
            {
                i.Startup();
            }
        }

        #endregion IMessageProducer
    }
}