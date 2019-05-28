using Never.Hosting.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Never.Hosting
{
    /// <summary>
    /// 扩展
    /// </summary>
    public static class HostingExtension
    {
        public static Func<HostBuilderContext, IEnumerable<FileInfo>> ConfigFileBuilder(IEnumerable<string> fileName)
        {
            return new Func<HostBuilderContext, IEnumerable<FileInfo>>(builder =>
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
        /// Specify the startup type to be used by the host
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static HostBuilder UseStartup<T>(this HostBuilder builder, Func<HostBuilderContext, IEnumerable<FileInfo>> jsonConfigFiles = null) where T : AppStartup
        {
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

                builder.Properties["Never.Hosting.Extension.IConfigurationBuilder"] = g;
            });

            //config ioc
            builder.ConfigureServices((h, i) =>
            {
                var ctors = from n in typeof(T).GetConstructors(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                            let parameters = n.GetParameters()
                            let length = parameters.Length
                            select new { ctor = n, parameters = parameters, length = length };

                foreach (var ctor in ctors.OrderByDescending(t => t.length))
                {
                    var paramters = new List<object>(ctor.length);
                    foreach (var p in ctor.parameters)
                    {
                        if (p.ParameterType == typeof(HostBuilderContext))
                        {
                            paramters.Add(h);
                            continue;
                        }
                        if (p.ParameterType == typeof(IServiceCollection))
                        {
                            paramters.Add(i);
                            continue;
                        }
                        if (p.ParameterType == typeof(IHostingEnvironment))
                        {
                            paramters.Add(h.HostingEnvironment);
                            continue;
                        }
                        if (p.ParameterType == typeof(IConfiguration))
                        {
                            paramters.Add(h.Configuration);
                            continue;
                        }
                        if (p.ParameterType == typeof(IConfigurationBuilder))
                        {
                            var config = builder.Properties["Never.Hosting.Extension.IConfigurationBuilder"] as IConfigurationBuilder;
                            builder.Properties.Remove("Never.Hosting.Extension.IConfigurationBuilder");
                            paramters.Add(config);
                            continue;
                        }

                        throw new Exception(string.Format("the parameter type {0} can not resolve in {1} cotrs", p.ParameterType, typeof(T)));
                    }


                    var start = (T)ctor.ctor.Invoke(paramters.ToArray());
                    if (start == null)
                        return;

                    var provider = start.ConfigureServices(i);

                    //config serviceprivider
                    builder.UseServiceProviderFactory(new ServiceProviderFactory(provider));
                }
            });

            return builder;
        }

        /// <summary>
        /// 使用注入
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="services"></param>
        /// <returns></returns>
        public static ApplicationStartup UseHostingDependency(this ApplicationStartup startup, IServiceCollection services)
        {
            if (startup.Items.ContainsKey("UseConsoleHostingDependency"))
                return startup;

            startup.Items["UseConsoleHostingDependency"] = "t";
            return new ApiServiceCollection(services).Populate(startup);
        }
    }

    /// <summary>
    /// service provider
    /// </summary>
    internal class ServiceProviderFactory : IServiceProviderFactory<IServiceProvider>
    {
        private readonly IServiceProvider serviceProvider = null;
        public ServiceProviderFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IServiceProvider CreateBuilder(IServiceCollection services)
        {
            return this.serviceProvider;
        }

        public IServiceProvider CreateServiceProvider(IServiceProvider serviceProvider)
        {
            return this.serviceProvider;
        }
    }
}
