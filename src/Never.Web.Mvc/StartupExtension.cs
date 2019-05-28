using Never.Web.Mvc.Dispatcher;
using Never.Web.Mvc.IoC;
using Never.Web.Mvc.DataAnnotations;
using System;
using System.Collections.Generic;
using System.IO;

#if !NET461
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Never.Web.Mvc
{
    /// <summary>
    /// 启动扩展
    /// </summary>
    public static class StartupExtension
    {
        #region mvc

        /// <summary>
        /// 启动mvc Ioc注入支持
        /// </summary>
        /// <param name="startup"></param>
        /// <returns></returns>
        public static ApplicationStartup UseMvcDependency(this ApplicationStartup startup, IServiceCollection services)
        {
            if (startup.Items.ContainsKey("UseMvcDependency"))
                return startup;

            startup.Items["UseMvcDependency"] = "t";
            startup.RegisterStartService(new ApiStartService());
            return new ApiServiceCollection(services).Populate(startup);
        }

        /// <summary>
        /// 启动mvc统一接口资源请求
        /// </summary>
        /// <param name="startup">启动环境</param>
        /// <returns></returns>
        public static ApplicationStartup UseMvcActionCustomRoute(this ApplicationStartup startup, IServiceCollection services)
        {
            if (startup.Items.ContainsKey("UseMvcActionCustomRoute"))
                return startup;

            startup.Items["UseMvcActionCustomRoute"] = "t";
            services.Configure<Microsoft.AspNetCore.Mvc.MvcOptions>((x) =>
            {
                x.Conventions.Add(new CustomHttpControllerSelector());
            });

            return startup;
        }

        /// <summary>
        /// 启动mvc的ModelState验证
        /// </summary>
        /// <param name="startup"></param>
        /// <returns></returns>
        public static ApplicationStartup UseMvcModelStateValidation(this ApplicationStartup startup)
        {
            if (startup.Items.ContainsKey("UseMvcModelStateValidation"))
                return startup;

            startup.RegisterStartService(new StartupService());
            startup.Items["UseMvcModelStateValidation"] = "t";
            return startup;
        }

        /// <summary>
        /// 启动mvc的ModelState验证
        /// </summary>
        /// <param name="startup"></param>
        /// <returns></returns>
        [System.Obsolete("该方法会返回400，未能解决")]
        public static ApplicationStartup UseMvcModelStateValidation(this ApplicationStartup startup, Microsoft.Extensions.DependencyInjection.IServiceCollection services)
        {
            if (startup.Items.ContainsKey("UseMvcModelStateValidation"))
                return startup;

            services.Configure<Microsoft.AspNetCore.Mvc.MvcOptions>(opt =>
            {
                opt.ModelValidatorProviders.Add(new ValidatorProvider());
            });

            startup.RegisterStartService(new StartupService());
            startup.Items["UseMvcModelStateValidation"] = "t";
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

        #endregion mvc
    }
}

#else

using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;

namespace Never.Web.Mvc
{
    /// <summary>
    /// 启动扩展
    /// </summary>
    public static class StartupExtension
    {
#region mvc

        /// <summary>
        /// 启动mvc Ioc注入支持
        /// </summary>
        /// <param name="startup"></param>
        /// <returns></returns>
        public static ApplicationStartup UseMvcDependency(this ApplicationStartup startup)
        {
            System.Web.Mvc.DependencyResolver.SetResolver(new IoC.DependencyResolver());
            startup.RegisterStartService(new IoC.ApiStartService());
            return startup;
        }

        /// <summary>
        /// 启动mvc统一接口资源请求
        /// </summary>
        /// <param name="startup">启动环境</param>
        /// <returns></returns>
        public static ApplicationStartup UseMvcActionCustomRoute(this ApplicationStartup startup)
        {
            return UseMvcActionCustomRoute(startup, RouteTable.Routes);
        }

        /// <summary>
        /// 启动mvc统一接口资源请求
        /// </summary>
        /// <param name="startup">启动环境</param>
        /// <param name="routes">The routes.</param>
        /// <returns></returns>
        public static ApplicationStartup UseMvcActionCustomRoute(this ApplicationStartup startup, RouteCollection routes)
        {
            var controller = new CustomHttpControllerSelector(routes);
            // ControllerBuilder.Current.SetControllerFactory(controller);
            startup.RegisterStartService(controller);
            return startup;
        }

        /// <summary>
        /// 启动mvc的ModelState验证
        /// </summary>
        /// <param name="startup"></param>
        /// <returns></returns>
        public static ApplicationStartup UseMvcModelStateValidation(this ApplicationStartup startup)
        {
            if (startup.Items.ContainsKey("UseMvcModelStateValidation"))
                return startup;

            System.Web.Mvc.ModelValidatorProviders.Providers.Add(new ValidatorProvider());
            startup.RegisterStartService(new StartupService());

            startup.Items["UseMvcModelStateValidation"] = "t";
            return startup;
        }

#endregion mvc
    }
}

#endif