#if !NET461
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Web.WebApi.IoC
{
    public class ApiServiceScope : IServiceScope
    {
        private readonly Never.IoC.ILifetimeScope scope = null;

        public ApiServiceScope(Never.IoC.ILifetimeScope scope)
        {
            this.scope = scope;
            this.ServiceProvider = new ApiServiceProvider(scope);
        }

        public IServiceProvider ServiceProvider { get; }

        public void Dispose()
        {
            scope.Dispose();
        }
    }
}
#endif