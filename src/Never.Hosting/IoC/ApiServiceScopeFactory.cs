#if !NET461
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Hosting.IoC
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Microsoft.Extensions.DependencyInjection.IServiceScopeFactory" />
    public class ApiServiceScopeFactory : IServiceScopeFactory
    {
        private readonly Never.IoC.IServiceLocator serviceLocator = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiServiceScopeFactory" /> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        public ApiServiceScopeFactory(Never.IoC.IServiceLocator serviceLocator)
        {
            this.serviceLocator = serviceLocator;
        }

        /// <summary>
        /// Create an <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceScope" /> which
        /// contains an <see cref="T:System.IServiceProvider" /> used to resolve dependencies from a
        /// newly created scope.
        /// </summary>
        /// <returns>
        /// An <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceScope" /> controlling the
        /// lifetime of the scope. Once this is disposed, any scoped services that have been resolved
        /// from the <see cref="P:Microsoft.Extensions.DependencyInjection.IServiceScope.ServiceProvider" />
        /// will also be disposed.
        /// </returns>
        public IServiceScope CreateScope()
        {
            return new ApiServiceScope(this.serviceLocator.BeginLifetimeScope());
        }
    }
}
#endif