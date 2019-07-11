using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Never;
using Never.Configuration;
using Never.IoC;
using Never.Startups;
using Never.Startups.Impls;
using Never.Web;
using Never.Web.Caching;
using Never.Web.IoC.Providers;
using Never.Web.Mvc;

namespace Never.TestMvc
{
    public class MvcApplication : Never.Web.Mvc.WebHttpApplication
    {
        public MvcApplication() : base(() => new WebApplicationStartup() { })
        {
            this.OnStarting += this.MvcApplication_OnStarting;
        }

        private void MvcApplication_OnStarting(object sender, Never.StartupEventArgs e)
        {
            var configReader = new AppConfigReader(Never.Configuration.MachineConfig.WebConfig);
            e.Startup.RegisterAssemblyFilter("Never".CreateAssemblyFilter())
            .UseEasyIoC((x, y, z) =>
            {
                x.RegisterType<MyDispose, MyDispose>(string.Empty, IoC.ComponentLifeStyle.Scoped);
            })
            .UseConcurrentCache()
            .UseDataContractJson()
            .UseEasyJson()
            .UseAppConfig(configReader)
            .UseMvcModelStateValidation()
            .UseMvcActionCustomRoute()
            .UseInprocEventProviderCommandBus<Never.Commands.DefaultCommandContext>()
            .UseMvcDependency()
            .Startup();
        }

        protected override void Application_EndRequest(object sender, EventArgs e)
        {
            base.Application_EndRequest(sender, e);
        }

        protected override void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            base.Application_Start();
        }

        public class MyDispose : IDisposable
        {
            public MyDispose()
            {
            }

            public void Dispose()
            {
            }
        }
    }
}
