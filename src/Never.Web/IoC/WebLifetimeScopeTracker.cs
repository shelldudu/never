using Never.Caching;
using Never.IoC;
using Never.Web.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Web.IoC
{
    /// <summary>
    /// web的生命周期
    /// </summary>
    /// <seealso cref="Never.IoC.DefaultLifetimeScopeTracker" />
    public class WebLifetimeScopeTracker : DefaultLifetimeScopeTracker
    {
        #region ctor

        /// <summary>
        /// Initializes the <see cref="WebLifetimeScopeTracker"/> class.
        /// </summary>
        static WebLifetimeScopeTracker()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebLifetimeScopeTracker"/> class.
        /// </summary>
        public WebLifetimeScopeTracker()
        {
        }

        #endregion ctor

        /// <summary>
        /// 开始一个范围
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public override ILifetimeScope StartScope(ILifetimeScope parent)
        {
            return new HttpThreadCache().Get("BeginLifetimeScope", () => base.StartScope(parent));
        }

        /// <summary>
        /// 结束所有范围
        /// </summary>
        public override void CleanScope()
        {
            var cache = new HttpThreadCache();
            var scope = cache.Get<ILifetimeScope>("BeginLifetimeScope");
            if (scope != null)
                scope.Dispose();

            cache.Remove("BeginLifetimeScope");
            base.CleanScope();
        }
    }
}