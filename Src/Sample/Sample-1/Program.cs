using Microsoft.Extensions.DependencyInjection;
using System;
using TinyService;
using Microsoft.Extensions.Logging;
using TinyService.Logging;
using TinyService.Core;
using Sample_1.Actors;
using System.Linq;
using TinyService.Cqrs;
using System.Reflection;
using Sample_1.Domain;
using Sample_1.CommandHandler;

namespace Sample_1
{
    class Program
    {
        static void Main(string[] args)
        {
            IServiceProvider serviceProvider = BuildServiceCollection().BuildServiceProvider();
           
            Log.SetLoggerFactory(serviceProvider.GetService<ILoggerFactory>());

            var factory =serviceProvider.GetService<IActorFactory>();

            var productactor= factory.GetActor<ProductActor>();

            Random random = new Random();

            Enumerable.Range(1, 1)
                         .Select(p =>
                         {
                             productactor.Tell(
                                 new CreateProductCommand(
                                 name: $"笔记本-{p}",
                                 category: "3C",
                                 summary: "这是一款高性能的笔记本",
                                 description: "Think Pad IBM",
                                 imageFile: $"http://www.think.com/{p}.jpg",
                                 price: p * random.Next(1, 100),
                                 status: ProductStatus.Draft)
                                 );

                             return p;
                         })
                         .ToList();




            productactor.Tell(new ProductActivateCommand(random.Next(1, 1), DateTime.Now));


            Console.ReadKey();
            
            
        }

        static IServiceCollection BuildServiceCollection()
        {
            var assemblys = new Assembly[] {
              Assembly.GetExecutingAssembly()
            };

            var services = new ServiceCollection();
            services.UseTinyService()
                    .AddDispatcher()
                    .AddEventHandlers(assemblys)
                    .AddCommandHandlers(assemblys)
                    .AddQueryHandlers(assemblys)
                    .AddSingleton<IInMemeoryRepository, InMemeoryRepository>()
                    .AddSingleton<IIdGenerator, IdGenerator>()
                    .AddLogging(log => log.AddConsole());

            return services;


        }
    }
}
