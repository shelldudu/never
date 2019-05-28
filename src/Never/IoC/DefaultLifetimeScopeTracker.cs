using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Never.Caching;

namespace Never.IoC
{
    /// <summary>
    /// 跟踪者
    /// </summary>
    public class DefaultLifetimeScopeTracker : ILifetimeScopeTracker
    {
        #region ctor

        /// <summary>
        ///
        /// </summary>
        public DefaultLifetimeScopeTracker()
        {
        }

        #endregion ctor

        #region ILifetimeScopeTracker

        /// <summary>
        /// 开始一个范围
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public virtual ILifetimeScope StartScope(ILifetimeScope parent)
        {
            return parent == null ? parent : parent.BeginLifetimeScope();
        }

        /// <summary>
        /// 结束所有范围
        /// </summary>
        public virtual void CleanScope()
        {
        }

        #endregion ILifetimeScopeTracker
    }
}