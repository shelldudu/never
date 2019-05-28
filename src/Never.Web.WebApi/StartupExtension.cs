using Never.IoC;
using Never.Startups;
using Never.Web.WebApi.Dispatcher;
using Never.Web.WebApi.Encryptions;
using Never.Web.WebApi.IoC;
using Never.Web.WebApi.MessageHandlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Never.Web.WebApi.DataAnnotations;

#if !NET461
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
#else

using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

#endif

namespace Never.Web.WebApi
{
    /// <summary>
    /// 启动扩展
    /// </summary>
    public static class StartupExtension
    {
        #region api

#if !NET461

        /// <summary>
        /// 启动api Ioc注入支持
        /// </summary>
        /// <param name="startup"></param>
        /// <returns></returns>
        public static ApplicationStartup UseApiDependency(this ApplicationStartup startup, IServiceCollection services)
        {
            if (startup.Items.ContainsKey("UseApiDependency"))
                return startup;

            startup.Items["UseApiDependency"] = "t";
            startup.RegisterStartService(new ApiStartService());
            return new ApiServiceCollection(services).Populate(startup);
        }

        /// <summary>
        /// 启动Api统一接口资源请求
        /// </summary>
        /// <param name="startup">启动环境</param>
        /// <returns></returns>
        public static ApplicationStartup UseApiActionCustomRoute(this ApplicationStartup startup, IServiceCollection services)
        {
            if (startup.Items.ContainsKey("UseApiActionCustomRoute"))
                return startup;

            services.AddMvcCore((x) =>
            {
                x.Conventions.Add(new CustomHttpControllerSelector());
            });

            startup.Items["UseApiActionCustomRoute"] = "t";
            return startup;
        }

        /// <summary>
        /// 启动Api的ModelState验证
        /// </summary>
        /// <param name="startup"></param>
        /// <returns></returns>
        public static ApplicationStartup UseApiModelStateValidation(this ApplicationStartup startup)
        {
            if (startup.Items.ContainsKey("UseApiModelStateValidation"))
                return startup;

            startup.RegisterStartService(new StartupService());
            startup.Items["UseApiModelStateValidation"] = "t";
            return startup;
        }

        /// <summary>
        /// 启动Api的ModelState验证
        /// </summary>
        /// <param name="startup"></param>
        /// <returns></returns>
        [System.Obsolete("该方法会返回400，未能解决")]
        public static ApplicationStartup UseApiModelStateValidation(this ApplicationStartup startup, Microsoft.Extensions.DependencyInjection.IServiceCollection services)
        {
            if (startup.Items.ContainsKey("UseApiModelStateValidation"))
                return startup;

            services.Configure<Microsoft.AspNetCore.Mvc.MvcOptions>(opt =>
            {
                opt.ModelValidatorProviders.Add(new ValidatorProvider());
            });

            startup.RegisterStartService(new StartupService());
            startup.Items["UseApiModelStateValidation"] = "t";
            return startup;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Func<Microsoft.AspNetCore.Hosting.WebHostBuilderContext, IEnumerable<FileInfo>> ConfigFileBuilder(IEnumerable<string> fileName)
        {
            return new Func<Microsoft.AspNetCore.Hosting.WebHostBuilderContext, IEnumerable<FileInfo>>(builder =>
            {
                var list = new List<FileInfo>();
                if (fileName.IsNullOrEmpty())
                    return list;

                foreach (var file in fileName)
                    list.Add(new System.IO.FileInfo(System.IO.Path.Combine(builder.HostingEnvironment.ContentRootPath, file)));

                return list;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="jsonConfigFiles"></param>
        /// <returns></returns>
        public static Microsoft.AspNetCore.Hosting.IWebHostBuilder UseJsonFileConfig(this Microsoft.AspNetCore.Hosting.IWebHostBuilder builder, Func<Microsoft.AspNetCore.Hosting.WebHostBuilderContext, IEnumerable<FileInfo>> jsonConfigFiles = null)
        {
            if (jsonConfigFiles == null)
                return builder;

            //config builder
            builder.ConfigureAppConfiguration((h, g) =>
            {
                var files = jsonConfigFiles?.Invoke(h);
                if (files.IsNotNullOrEmpty())
                {
                    foreach (var file in files)
                    {
                        if (file.Exists)
                            g.AddConfiguration(new ConfigurationBuilder().SetBasePath(h.HostingEnvironment.ContentRootPath).AddJsonFile(file.FullName, true, true).Build());
                        else
                            throw new System.IO.FileNotFoundException(string.Format("找不到文件{0}", file.FullName));
                    }
                }

                //if (File.Exists(Path.Combine(h.HostingEnvironment.ContentRootPath, "appsettings.json")))
                //    g.AddConfiguration(new ConfigurationBuilder().SetBasePath(h.HostingEnvironment.ContentRootPath).AddJsonFile("appsettings.json", true, true).Build());

                //if (File.Exists(Path.Combine(h.HostingEnvironment.ContentRootPath, $"appsettings.{h.HostingEnvironment.EnvironmentName}.json")))
                //    g.AddConfiguration(new ConfigurationBuilder().SetBasePath(h.HostingEnvironment.ContentRootPath).AddJsonFile($"appsettings.{h.HostingEnvironment.EnvironmentName}.json", true, true).Build());
            });

            return builder;
        }

        ///// <summary>
        ///// 启用自定义<seealso cref="System.Net.Http.HttpMethod"/>执行方法
        ///// </summary>
        ///// <param name="startup"></param>
        ///// <param name="keyInHeaderToRemark">在header中如何找到当前要重写的method方法</param>
        ///// <returns></returns>
        //public static ApplicationStartup UseApiHttpMethodOverride(this ApplicationStartup startup, string keyInHeaderToRemark = "X-HttpMethod-Override")
        //{
        //    return UseApiHttpMethodOverride(startup, GlobalConfiguration.Configuration, keyInHeaderToRemark);
        //}

        ///// <summary>
        ///// 启用自定义<seealso cref="System.Net.Http.HttpMethod"/>执行方法
        ///// </summary>
        ///// <param name="startup"></param>
        ///// <param name="configuration">The configuration.</param>
        ///// <param name="keyInHeaderToRemark">在header中如何找到当前要重写的method方法</param>
        ///// <returns></returns>
        //public static ApplicationStartup UseApiHttpMethodOverride(this ApplicationStartup startup, HttpConfiguration configuration, string keyInHeaderToRemark = "X-HttpMethod-Override")
        //{
        //    configuration.MessageHandlers.Add(new HttpMethodOverrideHandler(keyInHeaderToRemark));
        //    return startup;
        //}

        ///// <summary>
        ///// 启用api对数据解密
        ///// </summary>
        ///// <param name="startup"></param>
        ///// <param name="findSecurityModelCallback">指示如何在当前请求中找到相应的加密与解密的方法</param>
        ///// <returns></returns>
        //public static ApplicationStartup UseApiHttpSecurityHandler(this ApplicationStartup startup, IFunc<HttpRequestMessage, IEncryptContent> findSecurityModelCallback)
        //{
        //    return UseApiHttpSecurityHandler(startup, GlobalConfiguration.Configuration, findSecurityModelCallback);
        //}

        ///// <summary>
        ///// 启用api对数据解密
        ///// </summary>
        ///// <param name="startup"></param>
        ///// <param name="configuration">The configuration.</param>
        ///// <param name="findSecurityModelCallback">指示如何在当前请求中找到相应的加密与解密的方法</param>
        ///// <returns></returns>
        //public static ApplicationStartup UseApiHttpSecurityHandler(this ApplicationStartup startup, HttpConfiguration configuration, IFunc<HttpRequestMessage, IEncryptContent> findSecurityModelCallback)
        //{
        //    configuration.MessageHandlers.Insert(0, new SecurityMessageHandler(findSecurityModelCallback));
        //    return startup;
        //}

#else

        /// <summary>
        /// 启动api Ioc注入支持
        /// </summary>
        /// <param name="startup"></param>
        /// <returns></returns>
        public static ApplicationStartup UseApiDependency(this ApplicationStartup startup)
        {
            return UseApiDependency(startup, System.Web.Http.GlobalConfiguration.Configuration);
        }

        /// <summary>
        /// 启动api Ioc注入支持
        /// </summary>
        /// <param name="startup">The startup.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        public static ApplicationStartup UseApiDependency(this ApplicationStartup startup, HttpConfiguration configuration)
        {
            configuration.DependencyResolver = new DependencyResolver();
            startup.RegisterStartService(new ApiStartService());
            return startup;
        }

        /// <summary>
        /// 启动Api统一接口资源请求
        /// </summary>
        /// <param name="startup">启动环境</param>
        /// <returns></returns>
        public static ApplicationStartup UseApiActionCustomRoute(this ApplicationStartup startup)
        {
            return UseApiActionCustomRoute(startup, GlobalConfiguration.Configuration);
        }

        /// <summary>
        /// 启动Api统一接口资源请求
        /// </summary>
        /// <param name="startup">启动环境</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        public static ApplicationStartup UseApiActionCustomRoute(this ApplicationStartup startup, HttpConfiguration configuration)
        {
            var controller = new CustomHttpControllerSelector(configuration);
            var action = new CustomHttpActionSelector(controller);

            configuration.Services.Replace(typeof(IHttpControllerSelector), controller);
            configuration.Services.Replace(typeof(IHttpActionSelector), action);

            startup.RegisterStartService(controller);
            return startup;
        }

        /// <summary>
        /// 启用自定义<seealso cref="System.Net.Http.HttpMethod"/>执行方法
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="keyInHeaderToRemark">在header中如何找到当前要重写的method方法</param>
        /// <returns></returns>
        public static ApplicationStartup UseApiHttpMethodOverride(this ApplicationStartup startup, string keyInHeaderToRemark = "X-HttpMethod-Override")
        {
            return UseApiHttpMethodOverride(startup, GlobalConfiguration.Configuration, keyInHeaderToRemark);
        }

        /// <summary>
        /// 启用自定义<seealso cref="System.Net.Http.HttpMethod"/>执行方法
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="keyInHeaderToRemark">在header中如何找到当前要重写的method方法</param>
        /// <returns></returns>
        public static ApplicationStartup UseApiHttpMethodOverride(this ApplicationStartup startup, HttpConfiguration configuration, string keyInHeaderToRemark = "X-HttpMethod-Override")
        {
            configuration.MessageHandlers.Add(new HttpMethodOverrideHandler(keyInHeaderToRemark));
            return startup;
        }

        /// <summary>
        /// 启用api对数据解密
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="findSecurityModelCallback">指示如何在当前请求中找到相应的加密与解密的方法</param>
        /// <returns></returns>
        public static ApplicationStartup UseApiHttpSecurityHandler(this ApplicationStartup startup, Func<HttpRequestMessage, IApiContentEncryptor> findSecurityModelCallback)
        {
            return UseApiHttpSecurityHandler(startup, GlobalConfiguration.Configuration, findSecurityModelCallback);
        }

        /// <summary>
        /// 启用api对数据解密
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="findSecurityModelCallback">指示如何在当前请求中找到相应的加密与解密的方法</param>
        /// <returns></returns>
        public static ApplicationStartup UseApiHttpSecurityHandler(this ApplicationStartup startup, HttpConfiguration configuration, Func<HttpRequestMessage, IApiContentEncryptor> findSecurityModelCallback)
        {
            configuration.MessageHandlers.Insert(0, new SecurityMessageHandler(findSecurityModelCallback));
            return startup;
        }

        /// <summary>
        /// 启动Api的ModelState验证
        /// </summary>
        /// <param name="startup"></param>
        /// <returns></returns>
        public static ApplicationStartup UseApiModelStateValidation(this ApplicationStartup startup)
        {
            if (startup.Items.ContainsKey("UseApiModelStateValidation"))
                return startup;

            System.Web.Http.GlobalConfiguration.Configuration.Services.Add(typeof(System.Web.Http.Validation.ModelValidatorProvider), new ValidatorProvider());
            startup.RegisterStartService(new StartupService());
            startup.Items["UseApiModelStateValidation"] = "t";
            return startup;
        }

#endif

        #endregion api
    }
}