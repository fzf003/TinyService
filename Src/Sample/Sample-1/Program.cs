using Microsoft.Extensions.DependencyInjection;
using System;
using TinyService;
using Microsoft.Extensions.Logging;
using TinyService.Logging;
using TinyService.Core;
using Sample_1.Actors;
using System.Linq;

namespace Sample_1
{
    class Program
    {
        static void Main(string[] args)
        {
            IServiceProvider serviceProvider = BuildServiceCollection().BuildServiceProvider();
            //Log.SetLoggerFactory(serviceProvider.GetService<TinyService.Logging.ILoggerFactory>());

            var factory =serviceProvider.GetService<IActorFactory>();

            var useractor= factory.GetActor<UserActor>("user");

            
            Enumerable.Range(1, 10)
                      .Select(p => { 
                          useractor.Tell(p.ToString()); 
                          return p; 
                      })
                      .ToList();


            Console.ReadKey();
        }

        static IServiceCollection BuildServiceCollection()
        {
            var services = new ServiceCollection();
            services.UseTinyService()
                    .AddLogging(log => log.AddConsole());

            return services;


        }
    }
}
