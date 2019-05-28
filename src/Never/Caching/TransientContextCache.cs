using System;
using System.Collections;

namespace Never.Caching
{
    /// <summary>
    /// 短暂的上下文缓存
    /// </summary>
    public sealed class TransientContextCache : ContextCache, ICaching, IDisposable
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadContextCache"/> class.
        /// </summary>
        public TransientContextCache()
            : base(new Hashtable())
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