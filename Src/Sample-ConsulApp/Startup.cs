using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TinyService.Discovery.Consul;

namespace Sample_ConsulApp
{
    public class Startup
    {
 
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddOptions();
            services.AddDiscovery();

          
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
             app.UseDiscovery();

            
        }
    }
}
