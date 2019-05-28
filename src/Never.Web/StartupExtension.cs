#if !NET461
#else

using Never.Caching;
using Never.Commands;
using Never.CommandStreams;
using Never.Events;
using Never.EventStreams;
using Never.IoC;
using Never.Logging;
using Never.Messages;
using Never.Serialization;
using Never.Startups;
using Never.Startups.Impls;
using Never.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Never.Web
{
    /// <summary>
    /// 启动扩展
    /// </summary>
    public static partial class StartupExtension
    {
        #region Serializer

        /// <summary>
        /// 启动Serializer支持
        /// </summary>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <returns></returns>
        public static ApplicationStartup UseJavaScriptSerializer(this ApplicationStartup startup)
        {
            return StartupHelper.UseJavaScriptSerializer(startup);
        }

        #endregion Serializer

        #region web

        /// <summary>
        /// 启动wcf Ioc注入支持
        /// </summary>
        /// <param name="startup"></param>
        /// <returns></returns>
        public static ApplicationStartup UseWebEnvironment(this ApplicationStartup startup)
        {
            return StartupHelper.UseWebEnvironment(startup);
        }

        #endregion web

        /// <summary>
        /// 启动MemoryCache支持
        /// </summary>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <returns></returns>
        public static ApplicationStartup UseMemoryCache(this ApplicationStartup startup)
        {
            return StartupHelper.UseMemoryCache(startup);
        }

        /// <summary>
        /// 启动MemoryCache支持
        /// </summary>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <param name="key">IoC容器中的key</param>
        /// <returns></returns>
        public static ApplicationStartup UseMemoryCache(this ApplicationStartup startup, string key)
        {
            return StartupHelper.UseMemoryCache(startup, key);
        }

        /// <summary>
        /// 启动HttpRequestCache支持
        /// </summary>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <returns></returns>
        public static ApplicationStartup UseHttpRequestCache(this ApplicationStartup startup)
        {
            return StartupHelper.UseHttpRequestCache(startup);
        }

        /// <summary>
        /// 启动HttpRequestCache支持
        /// </summary>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <param name="key">IoC容器中的key</param>
        /// <returns></returns>
        public static ApplicationStartup UseHttpRequestCache(this ApplicationStartup startup, string key)
        {
            return StartupHelper.UseHttpRequestCache(startup, key);
        }

        /// <summary>
        /// 启动RuntimeCache支持
        /// </summary>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <returns></returns>
        public static ApplicationStartup UseHttpRuntimeCache(this ApplicationStartup startup)
        {
            return StartupHelper.UseHttpRuntimeCache(startup);
        }

        /// <summary>
        /// 启动RuntimeCache支持
        /// </summary>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <param name="key">IoC容器中的key</param>
        /// <returns></returns>
        public static ApplicationStartup UseHttpRuntimeCache(this ApplicationStartup startup, string key)
        {
            return StartupHelper.UseHttpRuntimeCache(startup, key);
        }
    }
}

#endif