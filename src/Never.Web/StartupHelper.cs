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
using Never.Web.Caching;
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
    public static partial class StartupHelper
    {
        #region caching

        /// <summary>
        /// 启动Serializer支持
        /// </summary>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <returns></returns>
        public static ApplicationStartup UseJavaScriptSerializer(ApplicationStartup startup)
        {
            if (startup.ServiceRegister == null)
                return startup;

            if (startup.Items.ContainsKey("UseJavaScriptSerializer"))
                return startup;

            if (startup.Items.ContainsKey("UseJavaScriptSerializer"))
                return startup;

            startup.ServiceRegister.RegisterType(typeof(Never.Web.Serialization.JavaScriptSerializer), typeof(IJsonSerializer), GlobalConstantSetting.SerializerKey.JavaScript, ComponentLifeStyle.Singleton);

            startup.Items["UseJavaScriptSerializer"] = "t";
            return startup;
        }

        /// <summary>
        /// 启动RuntimeCache支持
        /// </summary>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <returns></returns>
        public static ApplicationStartup UseHttpRuntimeCache(ApplicationStartup startup)
        {
            return UseHttpRuntimeCache(startup, string.Empty);
        }

        /// <summary>
        /// 启动RuntimeCache支持
        /// </summary>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <param name="key">IoC容器中的key</param>
        /// <returns></returns>
        public static ApplicationStartup UseHttpRuntimeCache(ApplicationStartup startup, string key)
        {
            if (startup.ServiceRegister == null)
                return startup;

            if (startup.Items.ContainsKey("UseHttpRuntimeCache" + key))
                return startup;

            startup.ServiceRegister.RegisterType(typeof(HttpRuntimeCache), typeof(ICaching), key, ComponentLifeStyle.Singleton);
            startup.Items["UseHttpRuntimeCache" + key] = "t";

            return startup;
        }

        /// <summary>
        /// 启动HttpRequestCache支持
        /// </summary>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <returns></returns>
        public static ApplicationStartup UseHttpRequestCache(ApplicationStartup startup)
        {
            return UseHttpRequestCache(startup, string.Empty);
        }

        /// <summary>
        /// 启动HttpRequestCache支持
        /// </summary>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <param name="key">IoC容器中的key</param>
        /// <returns></returns>
        public static ApplicationStartup UseHttpRequestCache(ApplicationStartup startup, string key)
        {
            if (startup.ServiceRegister == null)
                return startup;

            if (startup.Items.ContainsKey("UseHttpRequestCache" + key))
                return startup;

            startup.ServiceRegister.RegisterType(typeof(HttpRequestCache), typeof(ICaching), key, ComponentLifeStyle.Scoped);
            startup.Items["UseHttpRequestCache" + key] = "t";
            return startup;
        }

        /// <summary>
        /// 启动MemoryCache支持
        /// </summary>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <returns></returns>
        public static ApplicationStartup UseMemoryCache(ApplicationStartup startup)
        {
            return UseMemoryCache(startup, string.Empty);
        }

        /// <summary>
        /// 启动MemoryCache支持
        /// </summary>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <param name="key">IoC容器中的key</param>
        /// <returns></returns>
        public static ApplicationStartup UseMemoryCache(ApplicationStartup startup, string key)
        {
            if (startup.ServiceRegister == null)
                return startup;

            if (startup.Items.ContainsKey("UseMemoryCache" + key))
                return startup;
            startup.ServiceRegister.RegisterType(typeof(Never.Web.Caching.MemoryCache), typeof(ICaching), key, ComponentLifeStyle.Singleton);
            startup.Items["UseMemoryCache" + key] = "t";
            return startup;
        }

        #endregion caching

        #region web

        /// <summary>
        /// 启动wcf Ioc注入支持
        /// </summary>
        /// <param name="startup"></param>
        /// <returns></returns>
        public static ApplicationStartup UseWebEnvironment(ApplicationStartup startup)
        {
            if (startup.Items.ContainsKey("UseWebEnvironment"))
                return startup;

            startup.ServiceRegister.RegisterType<Never.Web.Fakes.HttpContextWrapper, System.Web.HttpContextBase>(string.Empty, Never.IoC.ComponentLifeStyle.Scoped);
            startup.Items["UseWebEnvironment"] = "t";
            return startup;
        }

        #endregion web
    }
}

#endif