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
    public class ThreadLifetimeScopeTracker : DefaultLifetimeScopeTracker
    {
        #region ctor

        /// <summary>
        ///
        /// </summary>
        static ThreadLifetimeScopeTracker()
        {
        }

        /// <summary>
        ///
        /// </summary>
        public ThreadLifetimeScopeTracker()
        {
        }

        #endregion ctor

        /// <summary>
        ///
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override ILifetimeScope StartScope(ILifetimeScope parent)
        {
            return new ThreadContextCache().Get("BeginLifetimeScope", () => base.StartScope(parent));
        }

        /// <summary>
        ///
        /// </summary>
        public override void CleanScope()
        {
            var scope = new ThreadContextCache().Get<ILifetimeScope>("BeginLifetimeScope");
            if (scope != null)
                scope.Dispose();

            base.CleanScope();
        }
    }
}