#if !NET461
#else

using Never.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Never.Web.WebApi.IoC
{
    /// <summary>
    /// Api,Mvc的依赖注入
    /// </summary>
    public sealed class DependencyResolver : System.Web.Http.Dependencies.IDependencyResolver
    {
        #region webapi

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        System.Web.Http.Dependencies.IDependencyScope System.Web.Http.Dependencies.IDependencyResolver.BeginScope()
        {
            return this;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        object System.Web.Http.Dependencies.IDependencyScope.GetService(Type serviceType)
        {
            return ContainerContext.Current?.ServiceLocator?.ResolveOptional(serviceType);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        IEnumerable<object> System.Web.Http.Dependencies.IDependencyScope.GetServices(Type serviceType)
        {
            return ContainerContext.Current?.ServiceLocator?.ResolveAll(serviceType);
        }

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
            //不是放资源，请在在global里面的请求结束方法中调用如下方法：
            //ContainerContext.Current.LifetimeRelease.Dispose();
        }

        #endregion webapi
    }
}

#endif