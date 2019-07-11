#if !NET461
using Never.IoC.Injections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Web.WebApi
{
    /// <summary>
    /// 定义对 .NET Core 应用程序内所有应用程序对象公用的方法、属性和事件。
    /// </summary>
    public class WebStartup : Microsoft.AspNetCore.Hosting.IStartup
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="applicationStartup"></param>
        protected WebStartup(Func<ApplicationStartup> startup)
        {
            this.startup = startup;
        }

        /// <summary>
        /// 启动信息
        /// </summary>
        private readonly Func<ApplicationStartup> startup = null;

        /// <summary>
        /// 启动程序
        /// </summary>
        public ApplicationStartup Startup { get; private set; }

        /// <summary>
        /// 启动事件
        /// </summary>
        public StartupEventArgs StartupEvents { get; private set; }

        /// <summary>
        /// 正在启动
        /// </summary>
        public event EventHandler<StartupEventArgs> OnStarting;

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// it will ruturn null
        /// </summary>
        /// <param name="services"></param>
        public virtual IServiceProvider ConfigureServices(Microsoft.Extensions.DependencyInjection.IServiceCollection services)
        {
            this.Startup = this.startup();
            this.StartupEvents = new StartupEventArgs(this.Startup, services);
            if (this.OnStarting != null)
                this.OnStarting(this, this.StartupEvents);

            return new IoC.ApiServiceProvider(this.Startup.Startup().ServiceLocator.BeginLifetimeScope());
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public virtual void Configure(Microsoft.AspNetCore.Builder.IApplicationBuilder app)
        {
        }
    }
}

#endif