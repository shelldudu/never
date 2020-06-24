using Never.Attributes;
using Never.Commands;
using Never.EasySql;
using Never.EasySql.Client;
using Never.EventStreams;
using Never.IoC;
using Never.IoC.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Never.WorkerService;

namespace Never.WorkFlow.Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var build = CreateHostBuilder(args).Build();
            build.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                })
            .UseStartup<Startup>();
    }
}