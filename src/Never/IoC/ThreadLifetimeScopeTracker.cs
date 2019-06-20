using Never.Caching;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.IoC
{
    /// <summary>
    ///
    /// </summary>
    public class ThreadLifetimeScopeTracker : DefaultLifetimeScopeTracker, IDisposable
    {
        #region field and ctor

        /// <summary>
        /// 
        /// </summary>
        private readonly System.Threading.ThreadLocal<ILifetimeScope> threadLocal = null;

        /// <summary>
        ///
        /// </summary>
        public ThreadLifetimeScopeTracker()
        {
            this.threadLocal = new System.Threading.ThreadLocal<ILifetimeScope>(false);
        }

        #endregion ctor

        /// <summary>
        ///
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override ILifetimeScope StartScope(ILifetimeScope parent)
        {
            if (this.threadLocal.IsValueCreated)
                return this.threadLocal.Value;

            return this.threadLocal.Value = base.StartScope(parent);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void CleanScope()
        {
            if (this.threadLocal.IsValueCreated && this.threadLocal.Value != null)
            {
                this.threadLocal.Value.Dispose();
                this.threadLocal.Value = null;
            }

            base.CleanScope();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.threadLocal.Dispose();
        }
    }
}