#if !NET461
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Web.Mvc.IoC
{
    public class ApiServiceCollection
    {
        private readonly IServiceCollection serviceCollection = null;
        public ApiServiceCollection(IServiceCollection serviceCollection)
        {
            this.serviceCollection = serviceCollection;
        }

        static Never.IoC.ComponentLifeStyle convert(ServiceLifetime serviceLifetime)
        {
            switch (serviceLifetime)
            {
                case ServiceLifetime.Scoped:
                    return Never.IoC.ComponentLifeStyle.Scoped;

                case ServiceLifetime.Singleton:
                    return Never.IoC.ComponentLifeStyle.Singleton;

                case ServiceLifetime.Transient:
                    return Never.IoC.ComponentLifeStyle.Transient;
            }

            return Never.IoC.ComponentLifeStyle.Transient;
        }

        public ApplicationStartup Populate(ApplicationStartup startup)
        {
            startup.ServiceRegister.RegisterType<ApiServiceProvider, IServiceProvider>(string.Empty, Never.IoC.ComponentLifeStyle.Scoped);
            startup.ServiceRegister.RegisterType<ApiServiceScopeFactory, IServiceScopeFactory>(string.Empty, Never.IoC.ComponentLifeStyle.Singleton);

            foreach (var i in this.serviceCollection)
            {
                if (i.ImplementationInstance != null)
                {
                    startup.ServiceRegister.RegisterInstance(i.ImplementationInstance, i.ServiceType, string.Empty);
                    continue;
                }
                else if (i.ImplementationFactory != null && startup.ServiceRegister is Never.IoC.Injections.ServiceRegister)
                {
                    var @delegate = (Action<ServiceDescriptor, ApplicationStartup>)Delegate.CreateDelegate(typeof(Action<ServiceDescriptor, ApplicationStartup>), typeof(ApiServiceCollection).GetMethod("RegisterCallBack").MakeGenericMethod(new[] { i.ServiceType }));
                    @delegate.Invoke(i, startup);
                }
                else
                {
                    if (i.ImplementationType == null || i.ImplementationType.IsInterface)
                    {
                    }
                    else
                    {
                        startup.ServiceRegister.RegisterType(i.ImplementationType, i.ServiceType, string.Empty, convert(i.Lifetime));
                    }
                };
            }

            return startup;
        }

        public static void RegisterCallBack<T>(ServiceDescriptor serviceDescriptor, ApplicationStartup startup)
        {
            ((Never.IoC.Injections.ServiceRegister)startup.ServiceRegister).RegisterCallBack<T>(string.Empty, convert(serviceDescriptor.Lifetime), (x) =>
            {
                return (T)serviceDescriptor.ImplementationFactory.Invoke(new ApiServiceProvider(x));
            });
        }
    }
}
#endif