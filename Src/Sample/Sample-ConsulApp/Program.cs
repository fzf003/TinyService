using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TinyService.Discovery.Consul;

namespace Sample_ConsulApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
             CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)=>
               Host.CreateDefaultBuilder(args)
                       .ConfigureWebHostDefaults(webBuilder =>
                       {
                         webBuilder.AddDiscoveryOptions(GetConfiguration()) 
                                   .UseStartup<Startup>();
                       });
                
                   
        

        static IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                               .SetBasePath(Directory.GetCurrentDirectory())
                               .AddJsonFile(path: "appsettings.json", optional: true, reloadOnChange: true)
                               .Build();
        }

    }
}
