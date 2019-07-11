using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Never.Configuration;
using Never.Web.WebApi;
using System;
using System.Threading.Tasks;

namespace Never.TestWebApi
{
    public class Startup : Never.Web.WebApi.WebStartup
    {
        public IConfiguration Configuration { get; private set; }
        public IHostingEnvironment Environment { get; private set; }
        public IServiceProvider autofac { get; set; }
        public IServiceProvider easyioc { get; set; }

        public Startup(IConfiguration configuration, IHostingEnvironment environment) : base(() => new Never.Web.WebApplicationStartup())
        {
            this.Environment = environment;
            this.Configuration = configuration;

            this.OnStarting += this.Startup_OnStarting;
        }

        private void Startup_OnStarting(object sender, StartupEventArgs e)
        {
            e.Startup.RegisterAssemblyFilter("Never".CreateAssemblyFilter())
            .UseEasyIoC((x, y, z) =>
            {
                x.RegisterType<MyDispose, MyDispose>(string.Empty, IoC.ComponentLifeStyle.Scoped);
            })
            .UseConcurrentCache()
            .UseDataContractJson()
            .UseEasyJson()
            .UseAppConfig(this.Configuration)
            .UseApiModelStateValidation()
            .UseApiActionCustomRoute(e.Collector as IServiceCollection)
            .UseInprocEventProviderCommandBus<Never.Commands.DefaultCommandContext>()
            .UseApiDependency(e.Collector as IServiceCollection)
            .Startup();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public override IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(r => { });
            services.AddScoped<ApiServiceScopeClearMiddleware>();
            var provider = base.ConfigureServices(services);
            return provider;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public override void Configure(IApplicationBuilder app)
        {
            app.UseExceptionHandler("/Error");

            app.UseStaticFiles();
            app.UseMvc(t =>
            {
            });

            app.UseMiddleware<ApiServiceScopeClearMiddleware>();
            //var p = autofac.GetService<IOptions<Microsoft.AspNetCore.Mvc.MvcOptions>>();
            //var e = easyioc.GetService<IOptions<Microsoft.AspNetCore.Mvc.MvcOptions>>();
            //var te = autofac.GetService<Microsoft.ApplicationInsights.Channel.ITelemetryChannel>();
            //te = easyioc.GetService<Microsoft.ApplicationInsights.Channel.ITelemetryChannel>();
        }
    }

    public class ApiServiceScopeClearMiddleware : IMiddleware
    {
        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            return next.Invoke(context);
        }
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