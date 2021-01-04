using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Topshelf;
using WebAPI.Extensions;

namespace WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //CreateHostBuilder(args).Build().Run();

            var builder = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json");
            IConfigurationRoot configuration = builder.Build();

            var serviceProvider = new ServiceCollection()
                    .AddSingleton(typeof(ILogger<>), typeof(Logger<>))
                    .AddLogging((logingbuilder) =>
                    {
                        logingbuilder.AddConsole();

#if (DEBUG)
                        logingbuilder.AddLog4Net(log4NetConfigFile: "log4net.debug.config");
#else
                       logingbuilder.AddLog4Net(log4NetConfigFile: "log4net.config");     
#endif

                        logingbuilder.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
                    })
                    .BuildServiceProvider();
            var _logger = serviceProvider.GetService<ILogger<Program>>();
            _logger.LogInformation("AtasphereWebAPIService installation starting");

            HostFactory.Run(windowsService =>
            {
                windowsService.Service<WebAPIService>(s =>
                {
                    s.ConstructUsing(service => new WebAPIService());
                    s.WhenStarted(service => service.Start());
                    s.WhenStopped(service => service.Stop());
                });

                windowsService.RunAsLocalSystem();
                windowsService.StartAutomatically();

                windowsService.EnableServiceRecovery(rc =>
                {
                    rc.RestartService(1);
                    rc.OnCrashOnly();
                });

                windowsService.SetDescription("ATASphere WebAPI Service....");
                windowsService.SetDisplayName("ATASphere WebAPI Service");
                windowsService.SetServiceName("ATASphereWebAPIService");
                windowsService.OnException(ex => _logger.LogError(ex, ex.Message));
            });
        }

    }

    public class WebAPIService
    {
        public void Start()
        {
            CreateWebHostBuilder().Build().Start();
        }

        public void Stop()
        {
            using (ServiceController serviceController = new ServiceController("ATASphereWebAPIService"))
            {
                foreach (Process objProc in Process.GetProcessesByName("Atasphere.WebAPI.exe"))
                {
                    objProc.Kill();
                }
                serviceController.Stop();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder()
        {
            var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

            var webHost = WebHost.CreateDefaultBuilder().ConfigureLogging((context, builder) =>
            {
#if (DEBUG)
                builder.AddLog4Net(log4NetConfigFile: "log4net.debug.config");
#else
                builder.AddLog4Net(log4NetConfigFile: "log4net.config");     
#endif
            })
            //.UseUrls(config.GetValue<string>("URL"))
            .ConfigureKestrel(serverOptions =>
            {
                serverOptions.ConfigureHttps(16303);
                serverOptions.Limits.MaxConcurrentConnections = 10000000;
                serverOptions.Limits.MaxConcurrentUpgradedConnections = 10000000;
                serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(30);
                serverOptions.Limits.MaxRequestBodySize = null;
            })
            .UseStartup<Startup>();

            return webHost;
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                    .ConfigureLogging((context, builder) =>
                    {
#if (DEBUG)
                        builder.AddLog4Net(log4NetConfigFile: "log4net.debug.config");
#else
                builder.AddLog4Net(log4NetConfigFile: "log4net.config");     
#endif
                    })
                    .UseStartup<Startup>()
                    .Build();
    }
}
