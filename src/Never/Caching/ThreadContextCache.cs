using System;
using System.Collections;

namespace Never.Caching
{
    /// <summary>
    /// 当前线程请求缓存
    /// </summary>
    public sealed class ThreadContextCache : ContextCache, ICaching, IDisposable
    {
        #region field

        /// <summary>
        ///
        /// </summary>
        private readonly System.Threading.ThreadLocal<Hashtable> threadLocal = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadContextCache"/> class.
        /// </summary>
        public ThreadContextCache() : this(new System.Threading.ThreadLocal<Hashtable>(() => new Hashtable()))
        {
        }

        private ThreadContextCache(System.Threading.ThreadLocal<Hashtable> threadLocal) : base(threadLocal.Value)
        {
            this.threadLocal = threadLocal;
        }

        #endregion ctor

        #region IDisposable成员

        /// <summary>
        /// 释放内部资源
        /// </summary>
        /// <param name="isDispose">是否释放</param>
        protected override void Dispose(bool isDispose)
        {
            this.threadLocal.Value.Clear();
            this.threadLocal.Value = null;
            this.threadLocal.Dispose();
            base.Dispose(isDispose);
        }

        #endregion IDisposable成员
    }
}