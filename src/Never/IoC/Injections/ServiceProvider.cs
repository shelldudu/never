using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.IoC.Injections
{
    /// <summary>
    /// ser vice provider
    /// </summary>
    public class ServiceProvider : IServiceProvider
    {
        private readonly Never.IoC.ILifetimeScope scope = null;

        /// <summary>
        ///
        /// </summary>
        /// <param name="scope"></param>
        public ServiceProvider(Never.IoC.ILifetimeScope scope)
        {
            this.scope = scope;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public virtual object GetRequiredService(Type serviceType)
        {
            return this.scope.ResolveOptional(serviceType);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public virtual object GetService(Type serviceType)
        {
            return this.scope.ResolveOptional(serviceType);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T GetRequiredService<T>()
        {
            return (T)this.scope.ResolveOptional(typeof(T));
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T GetService<T>()
        {
            return (T)this.scope.ResolveOptional(typeof(T));
        }
    }
}