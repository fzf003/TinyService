using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;



namespace TinyService.Discovery.Consul
{
    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder AddDiscoveryOptions(this IWebHostBuilder webHostBuilder, IConfiguration configuration, int? runPort = null)
        {

            if (webHostBuilder == null)
            {
                throw new ArgumentNullException(nameof(webHostBuilder));
            }

            List<string> urls = new List<string>();

            ServiceConfig serviceConfig = new ServiceConfig();

            configuration.Bind("AppConfig", serviceConfig);

            if (runPort != null)
            {
                urls.Add($"http://*:{runPort}");
                serviceConfig.serviceUri = new Uri($"http://localhost:{runPort}");
            }
            else
            {
                var baseurl = UriConfiguration.GetUri();
                urls.Add($"http://*:{baseurl.Port}");
                serviceConfig.serviceUri = baseurl;
            }
            
            webHostBuilder.UseUrls(urls.ToArray());
            
            return webHostBuilder.ConfigureServices((context,services) => {
                services.Configure<ServiceConfig>(options => {
                    options.serviceName = serviceConfig.serviceName;
                    options.serviceUri = serviceConfig.serviceUri;
                    options.version = serviceConfig.version;
                    options.Tags = serviceConfig.Tags;
                });
            });

        }



        public static IApplicationBuilder UseDiscovery(this IApplicationBuilder appBuilder)
        {

            

            appBuilder.MapWhen(context => {
                return context.Request.Path.Value.Contains("/status");

            }, appbuild => {

                appbuild.Run(async context => {
                    var serviceconig = appBuilder.ApplicationServices.GetService<IOptions<ServiceConfig>>().Value;
                    await context.Response.WriteAsync($"serviceName:{serviceconig?.serviceName} -- serviceId:{serviceconig?.serviceId}");

                });

            });

            return appBuilder;
        }

    }
}
