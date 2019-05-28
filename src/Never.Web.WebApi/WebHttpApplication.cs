#if !NET461
#else

using Never.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.Web.WebApi
{
    /// <summary>
    /// 定义对 ASP.NET 应用程序内所有应用程序对象公用的方法、属性和事件。
    /// </summary>
    public abstract class WebHttpApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="startup"></param>
        protected WebHttpApplication(Func<ApplicationStartup> startup)
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
        /// 服务启动
        /// </summary>
        protected virtual void Application_Start()
        {
            this.Startup = this.startup();
            this.StartupEvents = new StartupEventArgs(this.Startup);
            if (this.OnStarting != null)
                OnStarting(this, this.StartupEvents);
        }

        /// <summary>
        /// 请求开始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 请求结束
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void Application_EndRequest(object sender, EventArgs e)
        {
            ContainerContext.Current?.ScopeTracker?.CleanScope();
        }
    }
}

#endif