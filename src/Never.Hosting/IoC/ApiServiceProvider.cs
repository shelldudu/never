using Microsoft.Extensions.DependencyInjection;
using Never.IoC.Injections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Hosting.IoC
{
    public class ApiServiceProvider : Never.IoC.Injections.ServiceProvider, ISupportRequiredService
    {
        private readonly Never.IoC.ILifetimeScope scope = null;

        public ApiServiceProvider(Never.IoC.ILifetimeScope scope) : base(scope)
        {
            this.scope = scope;
        }
    }
}