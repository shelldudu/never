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
    /// <seealso cref="Microsoft.Extensions.DependencyInjection.IServiceScope" />
    public class ApiServiceScope : IServiceScope
    {
        private readonly Never.IoC.ILifetimeScope scope = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiServiceScope" /> class.
        /// </summary>
        /// <param name="scope">The scope.</param>
        public ApiServiceScope(Never.IoC.ILifetimeScope scope)
        {
            this.scope = scope;
            this.ServiceProvider = new ApiServiceProvider(scope);
        }

        /// <summary>
        /// The <see cref="T:System.IServiceProvider" /> used to resolve dependencies from the scope.
        /// </summary>
        public IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            scope.Dispose();
        }
    }
}