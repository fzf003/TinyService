using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Steeltoe.Discovery.Client;

namespace Ordering.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        static IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                               .SetBasePath(Directory.GetCurrentDirectory())
                               .AddJsonFile(path: "appsettings.json", optional: true, reloadOnChange: true)
                               .Build();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .AddDiscoveryClient()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
              .ConfigureLogging(config => {
                  config.AddConsole(options =>
                  {
                      options.LogToStandardErrorThreshold = LogLevel.Information;
                  }).AddDebug();
                });
    }
}
