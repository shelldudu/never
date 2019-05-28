using Never.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Never.Hosting.IoC
{
    /// <summary>
    /// 
    /// </summary>
    public class ApiServiceProviderFactory : IServiceProviderFactory<IServiceScopeFactory>
    {
        private readonly ApplicationStartup applicationStartup = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiServiceProviderFactory"/> class.
        /// </summary>
        /// <param name="applicationStartup">The application startup.</param>
        public ApiServiceProviderFactory(ApplicationStartup applicationStartup)
        {
            this.applicationStartup = applicationStartup;
        }

        /// <summary>
        /// Creates a container builder from an <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
        /// </summary>
        /// <param name="services">The collection of services</param>
        /// <returns>
        /// A container builder that can be used to create an <see cref="T:System.IServiceProvider" />.
        /// </returns>
        public IServiceScopeFactory CreateBuilder(IServiceCollection services)
        {
            return new ApiServiceScopeFactory(this.applicationStartup.ServiceLocator);
        }

        /// <summary>
        /// Creates an <see cref="T:System.IServiceProvider" /> from the container builder.
        /// </summary>
        /// <param name="containerBuilder">The container builder</param>
        /// <returns>
        /// An <see cref="T:System.IServiceProvider" />
        /// </returns>
        public IServiceProvider CreateServiceProvider(IServiceScopeFactory containerBuilder)
        {
            return containerBuilder.CreateScope().ServiceProvider;
        }
    }
}
