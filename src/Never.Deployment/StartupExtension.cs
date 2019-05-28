using Never.Logging;
using System;
using System.Collections.Generic;

namespace Never.Deployment
{
    /// <summary>
    /// 启动扩展
    /// </summary>
    public static class StartupExtension
    {
        /// <summary>
        /// 启用Api路由健康报告信息
        /// </summary>
        /// <param name="startup">启动信息</param>
        /// <param name="secondInterval">每个A10文件的检查间隔，以秒为单位，最小时间为10秒</param>
        /// <param name="initApiRouteProviderMethod">初始化路由提供者</param>
        /// <returns></returns>
        public static ApplicationStartup UseApiUriRouteDispatch(this ApplicationStartup startup, int secondInterval, Func<IA10HealthReport, IEnumerable<IApiRouteProvider>> initApiRouteProviderMethod)
        {
            return UseApiUriRouteDispatch(startup, secondInterval, StartReport(), initApiRouteProviderMethod);
        }

        /// <summary>
        /// 启用Api路由健康报告信息
        /// </summary>
        /// <param name="startup">启动信息</param>
        /// <param name="secondInterval">每个A10文件的检查间隔，以秒为单位，最小时间为10秒</param>
        /// <param name="initApiRouteProviderMethod">初始化路由提供者</param>
        /// <param name="loggerBuilder">日志</param>
        /// <returns></returns>
        public static ApplicationStartup UseApiUriRouteDispatch(this ApplicationStartup startup, int secondInterval, Func<IA10HealthReport, IEnumerable<IApiRouteProvider>> initApiRouteProviderMethod, Func<ILoggerBuilder> loggerBuilder)
        {
            return UseApiUriRouteDispatch(startup, secondInterval, StartReport(), initApiRouteProviderMethod, loggerBuilder);
        }

        /// <summary>
        /// 启用Api路由健康报告信息
        /// </summary>
        /// <param name="startup">启动信息</param>
        /// <param name="a10HealthReport">A10报告</param>
        /// <param name="secondInterval">每个A10文件的检查间隔，以秒为单位，最小时间为10秒</param>
        /// <param name="initApiRouteProviderMethod">初始化路由提供者</param>
        /// <returns></returns>
        public static ApplicationStartup UseApiUriRouteDispatch(this ApplicationStartup startup, int secondInterval, IA10HealthReport a10HealthReport, Func<IA10HealthReport, IEnumerable<IApiRouteProvider>> initApiRouteProviderMethod)
        {
            startup.RegisterStartService(new StartupService(secondInterval < 10 ? 10 : secondInterval, initApiRouteProviderMethod, a10HealthReport));
            return startup;
        }

        /// <summary>
        /// 启用Api路由健康报告信息
        /// </summary>
        /// <param name="startup">启动信息</param>
        /// <param name="a10HealthReport">A10报告</param>
        /// <param name="secondInterval">每个A10文件的检查间隔，以秒为单位，最小时间为10秒</param>
        /// <param name="initApiRouteProviderMethod">初始化路由提供者</param>
        /// <param name="loggerBuilder">日志</param>
        /// <returns></returns>
        public static ApplicationStartup UseApiUriRouteDispatch(this ApplicationStartup startup, int secondInterval, IA10HealthReport a10HealthReport, Func<IA10HealthReport, IEnumerable<IApiRouteProvider>> initApiRouteProviderMethod, Func<ILoggerBuilder> loggerBuilder)
        {
            startup.RegisterStartService(new StartupService(secondInterval < 10 ? 10 : secondInterval, initApiRouteProviderMethod, a10HealthReport) { LoggerBuilder = loggerBuilder });
            return startup;
        }

        /// <summary>
        /// 开始一个健康报告
        /// </summary>
        /// <returns></returns>
        public static IA10HealthReport StartReport()
        {
            return new A10HealthReport();
        }
    }
}