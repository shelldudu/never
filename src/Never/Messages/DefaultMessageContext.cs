using System;
using System.Collections.Generic;

namespace Never.Messages
{
    /// <summary>
    /// 默认消息上下文
    /// </summary>
    public class DefaultMessageContext : IMessageContext, IDisposable
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMessageContext"/> class.
        /// </summary>
        public DefaultMessageContext()
        {
            this.Items = new System.Collections.Concurrent.ConcurrentDictionary<string, object>();
        }

        #endregion ctor

        #region iMessageContext

        /// <summary>
        /// 上下文集合
        /// </summary>
        public IDictionary<string, object> Items { get; private set; }

        /// <summary>
        /// 当前执行的对象类型
        /// </summary>
        public Type TargetType { get; internal set; }

        #endregion iMessageContext

        #region IDisposable

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="disposed"></param>
        protected virtual void Dispose(bool disposed)
        {
            if (!disposed)
                return;
        }

        #endregion IDisposable
    }
}