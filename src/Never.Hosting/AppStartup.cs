#if !NET461
using Never.IoC.Injections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Hosting
{
    /// <summary>
    /// 定义对 .NET Core 应用程序内所有应用程序对象公用的方法、属性和事件。
    /// </summary>
    public class AppStartup
    {
        /// <summary>
        /// 构造函数，目前构造参数只支持如下类型：
        /// <see cref="Microsoft.Extensions.Hosting.HostBuilderContext"/>
        /// <see cref="Microsoft.Extensions.DependencyInjection.IServiceCollection"/>
        /// <see cref="Microsoft.Extensions.Hosting.IHostingEnvironment"/>
        /// <see cref="Microsoft.Extensions.Configuration.IConfiguration"/>
        /// <see cref="Microsoft.Extensions.Configuration.IConfigurationBuilder"/>
        /// </summary>
        /// <param name="startup"></param>
        protected AppStartup(Func<ApplicationStartup> startup)
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
    }
}

#endif