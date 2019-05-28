using Never.Logging;
using Never.Startups;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Never.Deployment
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Never.Startups.IStartupService" />
    internal class StartupService : Never.Startups.IStartupService
    {
        #region field

        private readonly int secondInterval = 0;

        private readonly Func<IA10HealthReport, IEnumerable<IApiRouteProvider>> initApiRouteProvider = null;

        private readonly IA10HealthReport a10HealthReport = null;

        /// <summary>
        /// 日志信息
        /// </summary>
        public Func<ILoggerBuilder> LoggerBuilder { get; set; }

        #endregion field

        /// <summary>
        /// Initializes the <see cref="StartupService"/> class.
        /// </summary>
        static StartupService()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StartupService"/> class.
        /// </summary>
        public StartupService(int secondInterval, Func<IA10HealthReport, IEnumerable<IApiRouteProvider>> initApiRouteProvider, IA10HealthReport a10HealthReport)
        {
            this.secondInterval = secondInterval;
            this.initApiRouteProvider = initApiRouteProvider;
            this.a10HealthReport = a10HealthReport;
        }

        /// <summary>
        /// 排序，通常IoC的规则会排在前十，所以其他对象请在IoC后面
        /// </summary>
        int IStartupService.Order
        {
            get
            {
                /*最后再启动*/
                return 100;
            }
        }

        /// <summary>
        /// 在程序宿主环境开始启动时刻，要处理的逻辑
        /// </summary>
        /// <param name="context">启动上下文</param>
        void IStartupService.OnStarting(StartupContext context)
        {
            var providers = this.initApiRouteProvider == null ? null : this.initApiRouteProvider.Invoke(this.a10HealthReport);
            if (providers.IsNullOrEmpty())
                return;

            var type = typeof(ApiUriDispatcher<>);
            foreach (var p in providers)
            {
                var gtype = type.MakeGenericType(p.GetType());
                var instance = Activator.CreateInstance(gtype, p, this.a10HealthReport);
                if (this.LoggerBuilder != null && instance is IApiRouteLogTracker)
                    ((IApiRouteLogTracker)instance).LoggerBuilder = this.LoggerBuilder;

                context.ServiceRegister.RegisterInstance(instance, gtype);
            }

            a10HealthReport.Startup(this.secondInterval, providers);
        }
    }
}