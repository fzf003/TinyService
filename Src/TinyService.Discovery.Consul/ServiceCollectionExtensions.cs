using Consul;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TinyService.Discovery.Consul.Internal;

namespace TinyService.Discovery.Consul
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddDiscoveryClient(this IServiceCollection services, Action<ConsulClientConfiguration> cfgaction)
        {
            services.AddLogging();

            services.AddSingleton<IConsulClient>(new ConsulClient(cfg => {
                cfgaction(cfg);
            }));

            services.AddHttpClient();

             services.AddSingleton<IClusterProvider, ConsulHostedService>();

            services.AddSingleton<IClusterClinet, ClusterServiceClient>();

            return services;

        }


        public static IServiceCollection AddDiscovery(this IServiceCollection services)
        {
            services.AddLogging();

            services.AddSingleton<IConsulClient>(new ConsulClient(cfg => {
                cfg.Address = new Uri("http://localhost:8500");
            }));

            services.AddHttpClient();

            services.AddSingleton<IClusterProvider, ConsulHostedService>();

            services.AddSingleton<IClusterClinet, ClusterServiceClient>();

            services.AddHostedService<ConsulHostedService>();

            return services;

        }


    }
}
