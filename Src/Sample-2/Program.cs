using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Sample_2.Messages;
using Sample_2.Services;
using System;
using System.Net.Http;
using System.Net;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyService.ReactiveRabbit;
using TinyService.ReactiveRabbit.Brocker;
using TinyService.ReactiveRabbit.RabbitFactory;
using Newtonsoft.Json.Converters;

using Microsoft.Extensions.Http;

namespace Sample_2
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpClient httpClient = new HttpClient();

            IServiceProvider serviceProvider = BuildServiceCollection().BuildServiceProvider();

            var serviehost=serviceProvider.GetService<IServiceHost>();

            serviehost.Start();

            var obserable = Observable.Timer(TimeSpan.Zero,TimeSpan.FromMilliseconds(300))
                                      .Select(p => new RequestMessage(p + "--" + Guid.NewGuid().ToString("N")));

            obserable.Retry().Subscribe(serviehost.GetRequestEndPoint);

          



            Console.ReadKey();
        }


        static IServiceCollection BuildServiceCollection()
        {
            var services = new ServiceCollection();
            services.AddLogging(log => log.AddConsole());
            services.AddReactiveRabbit(setting =>
            {
                setting.HostName = "localhost";
                setting.Password = "fzf003";
                setting.UserName = "fzf003";
                setting.Port = 5672;
                setting.VirtualHost = "fzf003";
            });

            services.AddSingleton<IServiceHost, RequestProcessServiceHost>();
                

            return services;
        }

      
    }

    public static class ObservableExtensions
    {
        public static IObservable<TSource> TakeUntilInclusive<TSource>(this IObservable<TSource> source, Func<TSource, bool> predicate)
        {
            return Observable.Create<TSource>(
                observer => source.Subscribe(
                    item =>
                    {
                        observer.OnNext(item);
                        if (predicate(item))
                            observer.OnCompleted();
                    },
                    observer.OnError,
                    observer.OnCompleted
                    )
                );
        }
    }
}
