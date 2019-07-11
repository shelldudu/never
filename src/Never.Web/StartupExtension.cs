#if !NET461
#else

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
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
        /// <param name="key"></param>
        /// <returns></returns>
        public static ApplicationStartup UseJavaScriptSerializer(this ApplicationStartup startup, string key = "ioc.ser.javascript")
        {
            if (startup.ServiceRegister == null)
                return startup;

            if (startup.Items.ContainsKey("UseJavaScriptSerializer"))
                return startup;

            if (startup.Items.ContainsKey("UseJavaScriptSerializer"))
                return startup;

            startup.ServiceRegister.RegisterType(typeof(Never.Web.Serialization.JavaScriptSerializer), typeof(IJsonSerializer), key, ComponentLifeStyle.Singleton);

            startup.Items["UseJavaScriptSerializer"] = "t";
            return startup;
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
            if (startup.Items.ContainsKey("UseWebEnvironment"))
                return startup;

            startup.ServiceRegister.RegisterType<Never.Web.Fakes.HttpContextWrapper, System.Web.HttpContextBase>(string.Empty, Never.IoC.ComponentLifeStyle.Scoped);
            startup.Items["UseWebEnvironment"] = "t";
            return startup;
        }

        #endregion web

        #region cached

        /// <summary>
        /// 启动HttpRequestCache支持
        /// </summary>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <returns></returns>
        public static ApplicationStartup UseHttpRequestCache(this ApplicationStartup startup)
        {
            return UseHttpRequestCache(startup, string.Empty);
        }

        /// <summary>
        /// 启动HttpRequestCache支持
        /// </summary>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <param name="key">IoC容器中的key</param>
        /// <returns></returns>
        public static ApplicationStartup UseHttpRequestCache(this ApplicationStartup startup, string key)
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
        /// 启动RuntimeCache支持
        /// </summary>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <returns></returns>
        public static ApplicationStartup UseHttpRuntimeCache(this ApplicationStartup startup)
        {
            return UseHttpRuntimeCache(startup, string.Empty);
        }

        /// <summary>
        /// 启动RuntimeCache支持
        /// </summary>
        /// <param name="startup">程序宿主环境配置服务</param>
        /// <param name="key">IoC容器中的key</param>
        /// <returns></returns>
        public static ApplicationStartup UseHttpRuntimeCache(this ApplicationStartup startup, string key)
        {
            if (startup.ServiceRegister == null)
                return startup;

            if (startup.Items.ContainsKey("UseHttpRuntimeCache" + key))
                return startup;

            startup.ServiceRegister.RegisterType(typeof(HttpRuntimeCache), typeof(ICaching), key, ComponentLifeStyle.Singleton);
            startup.Items["UseHttpRuntimeCache" + key] = "t";

            return startup;
        }

        #endregion
    }
}

#endif