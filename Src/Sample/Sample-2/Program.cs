using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Sample_2.Messages;
using Sample_2.Services;
using System;
using System.Net.Http;
using System.Reactive.Linq;
using TinyService.ReactiveRabbit;
using TinyService.ReactiveRabbit.RabbitFactory;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using TinyService.ReactiveRabbit.Brocker;

namespace Sample_2
{
    class Program
    {
        static void Main(string[] args)
        {
            IServiceProvider serviceProvider = BuildServiceCollection().BuildServiceProvider();

            var channelfactory = serviceProvider.GetService<IChannelFactory>();

            var brocker = serviceProvider.GetService<IMessageBroker>();

          brocker.RegisterHandle(queueName: "fzfrequest", onMessage: (c, p) =>
          {
              Console.WriteLine(Encoding.UTF8.GetString(c.RequestMessage.Body.Span));
              return Task.CompletedTask;
          });

            var inputendpoint = brocker.GetServiceEndPoint<string>(topicName: "MyRequestTopic", routingKey: "logs");


            var obserable = Observable.Timer(TimeSpan.Zero, TimeSpan.FromMilliseconds(300))
                                      .Select(p => p + "--" + Guid.NewGuid().ToString("N"));

            // obserable.Retry().Subscribe(inputendpoint);
               SendDelayMessage(brocker, channelfactory);
               Console.ReadKey();
         }





        static void SendDelayMessage(IMessageBroker messageBroker, IChannelFactory channelfactory)
        {
            var channel = channelfactory.Create();

            var exchangename = "fzf.delayed-9";

            ///声明交换机
            channel.DefineDelayExchange(exchangetype: ExchangeType.Direct, exchangename: exchangename);

            messageBroker.RegisterHandle(exchangeName: exchangename, queueName: "delayed.process", onMessage: (context, props) =>
            {
                Console.WriteLine("接受消息:{0}", Encoding.UTF8.GetString(context.RequestMessage.Body.Span));
                Console.WriteLine("接受时间:{0}", DateTime.Now.ToString());
                Console.WriteLine("========================================================");
                return Task.CompletedTask;
            });



            channel.SendDelayMessage(message:Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N")),exchangename:exchangename,delayseconds:5);

          

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
