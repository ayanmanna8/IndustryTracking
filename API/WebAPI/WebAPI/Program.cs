using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using Topshelf;

namespace WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Read appsettings.json
            var builder = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json");
            IConfigurationRoot configuration = builder.Build();


            var serviceProvider = new ServiceCollection()
                    .AddSingleton(typeof(ILogger<>), typeof(Logger<>))
                    //log4net 
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
            _logger.LogInformation("WebAPIService installation starting");

            //TOPSHElf
            HostFactory.Run(windowsService =>
            {
                windowsService.Service<WebAPIService>(s =>
                {
                    s.ConstructUsing(service => new WebAPIService());
                    s.WhenStarted(service => service.Start(args));
                    s.WhenStopped(service => service.Stop());
                });

                windowsService.RunAsLocalSystem();
                windowsService.StartAutomatically();

                windowsService.EnableServiceRecovery(rc =>
                {
                    rc.RestartService(1);
                    rc.OnCrashOnly();
                });

                windowsService.SetDescription("WebAPI Service....");
                windowsService.SetDisplayName("WebAPI Service");
                windowsService.SetServiceName("WebAPIService");
                windowsService.OnException(ex => _logger.LogError(ex, ex.Message));
            });
        }



    }

    public class WebAPIService
    {
        public void Start(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public void Stop()
        {
            using (ServiceController serviceController = new ServiceController("WebAPIService"))
            {
                foreach (Process objProc in Process.GetProcessesByName("WebAPI.exe"))
                {
                    objProc.Kill();
                }
                serviceController.Stop();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
          Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
              .ConfigureWebHostDefaults(webBuilder =>
              {
                  webBuilder.UseStartup<Startup>();
              });

        public static IWebHostBuilder CreateWebHostBuilder()
        {
            var curr = Directory.GetCurrentDirectory();
            var config = new ConfigurationBuilder()
        .SetBasePath(curr)
        .AddJsonFile("appsettings.json", optional: false)
        .Build();


            var webHost = new WebHostBuilder()
           .UseKestrel()
            .ConfigureLogging((logingbuilder) =>
            {
                logingbuilder.ClearProviders();
                logingbuilder.AddConsole();
#if (DEBUG)
                logingbuilder.AddLog4Net(log4NetConfigFile: "log4net.debug.config");
#else
                       logingbuilder.AddLog4Net(log4NetConfigFile: "log4net.config");     
#endif
                logingbuilder.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
            })
           .UseStartup<Startup>();

            return webHost;
        }
    }
}
