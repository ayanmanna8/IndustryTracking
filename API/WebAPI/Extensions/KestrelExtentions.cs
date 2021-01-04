using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace WebAPI.Extensions
{
     public static class KestrelExtentions
    {
        public static void ConfigureHttps(this KestrelServerOptions serverOptions, int port)
        {
            if (serverOptions is null)
            {
                throw new ArgumentNullException(nameof(serverOptions));
            }

            try
            {
                serverOptions.Listen(IPAddress.Any, port,
                listenOptions =>
                {
                    listenOptions.UseHttps(@".\localhost.pfx", "atasphere"); // Right now, I just hard coded password and pfx file path here, will create a user story
                });

                serverOptions.ConfigureHttpsDefaults(options =>
                {
                    options.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
                });

                X509Certificate2 cert = new X509Certificate2(@".\localhost.pfx", "atasphere", X509KeyStorageFlags.MachineKeySet);
                X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadWrite);
                store.Add(cert);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Failed to import certificate");
            }
        }

        public static void ConfigureHttp(this KestrelServerOptions serverOptions, int port)
        {
            if (serverOptions is null)
            {
                throw new ArgumentNullException(nameof(serverOptions));
            }

            try
            {
                serverOptions.Listen(IPAddress.Any, port);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Failed to import certificate");
            }
        }
    }
}
