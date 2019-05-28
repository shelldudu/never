using System;
using System.Collections;

namespace Never.Caching
{
    /// <summary>
    /// 当前线程请求缓存
    /// </summary>
    public class ThreadContextCache : ContextCache, ICaching, IDisposable
    {
        #region field

        /// <summary>
        ///
        /// </summary>
        protected readonly static System.Threading.ThreadLocal<Hashtable> CurrentThreadLocal = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadContextCache"/> class.
        /// </summary>
        static ThreadContextCache()
        {
            CurrentThreadLocal = new System.Threading.ThreadLocal<Hashtable>(() => new Hashtable());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadContextCache"/> class.
        /// </summary>
        public ThreadContextCache() : this(CurrentThreadLocal)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadContextCache"/> class.
        /// </summary>
        /// <param name="dictionary"></param>
        protected ThreadContextCache(IDictionary dictionary) : base(dictionary)
        {
        }

        private ThreadContextCache(System.Threading.ThreadLocal<Hashtable> threadLocal) : base(threadLocal.Value)
        {
        }

        #endregion ctor

        #region IDisposable成员

        /// <summary>
        /// 释放内部资源
        /// </summary>
        /// <param name="isDispose">是否释放</param>
        protected override void Dispose(bool isDispose)
        {
            base.Dispose(isDispose);
        }

        #endregion IDisposable成员
    }
}