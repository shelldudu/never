using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Never.Configuration;
using Microsoft.AspNetCore.Hosting.WindowsServices;

namespace Never.TestWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var host = WebHost.CreateDefaultBuilder(args);
            var pathToExe = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            var pathToContentRoot = Path.GetDirectoryName(pathToExe);
            host.UseKestrel((c,x) =>
            {
                var ports = c.Configuration["server.ports"];
                if (ports.IsNullOrEmpty())
                    return;

                foreach (var split in ports.Split(new char[] { ',', ';', ':' }).Select(t => t.AsInt()).Distinct())
                {
                    x.Listen(System.Net.IPAddress.Any, split);
                }
            });
            host.UseContentRoot(pathToContentRoot);
            host.UseStartup<Startup>();
            return host.Build();
        }
    }
}