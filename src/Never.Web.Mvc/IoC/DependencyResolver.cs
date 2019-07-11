#if NET461

using Never.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Never.Web.Mvc.IoC
{
    /// <summary>
    /// Api,Mvc的依赖注入
    /// </summary>
    public sealed class DependencyResolver : System.Web.Mvc.IDependencyResolver
    {
        #region mvc

        /// <summary>
        ///
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        object System.Web.Mvc.IDependencyResolver.GetService(Type serviceType)
        {
            return ContainerContext.Current?.ServiceLocator?.ResolveOptional(serviceType);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        IEnumerable<object> System.Web.Mvc.IDependencyResolver.GetServices(Type serviceType)
        {
            return ContainerContext.Current?.ServiceLocator?.ResolveAll(serviceType);
        }

        #endregion mvc
    }
}

#endif