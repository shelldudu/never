#if !NET461
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Web.WebApi.IoC
{
    public class ApiServiceScopeFactory : IServiceScopeFactory
    {
        private readonly Never.IoC.IServiceLocator serviceLocator = null;

        public ApiServiceScopeFactory(Never.IoC.IServiceLocator serviceLocator)
        {
            this.serviceLocator = serviceLocator;
        }

        public IServiceScope CreateScope()
        {
            return new ApiServiceScope(this.serviceLocator.BeginLifetimeScope());
        }
    }
}
#endif