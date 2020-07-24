using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sample_2.Messages;
using System;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyService.ReactiveRabbit;
using TinyService.ReactiveRabbit.Brocker;
using TinyService.ReactiveRabbit.RabbitFactory;

namespace Sample_2
{
    class Program
    {
        static void Main(string[] args)
        {
            const string exchangename = "PayService.RequestTopic";

            const string ququename = "PayService.ResponseConsumer";

            IServiceProvider serviceProvider = BuildServiceCollection().BuildServiceProvider();

            var broker= serviceProvider.GetService<IMessageBroker>();


            broker.RegisterHandle(exchangeName:exchangename, queueName: ququename,onMessage: context =>
             {
                 
                 var request = GetJsonPayload<RequestMessage>(context.RequestMessage);
                 Console.WriteLine(request.Payload);

                 return Task.CompletedTask;
             });

            

            var obserable = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(100))
                                      .Select(p => new RequestMessage(p + "--" + Guid.NewGuid().ToString("N")));

            obserable.Subscribe(broker.GetServicePublicEndPoint<RequestMessage>(exchangename));



            Console.ReadKey();
        }


        static IServiceCollection BuildServiceCollection()
        {
            var services = new ServiceCollection();
            services.AddLogging(log => log.AddConsole());
            services.AddReactiveRabbit(setting => {
                setting.HostName = "localhost";
                setting.Password = "fzf003";
                setting.UserName = "fzf003";
                setting.Port = 5672;
                setting.VirtualHost = "fzf003";
            });

            return services;
        }

        private static TResult GetJsonPayload<TResult>(PayloadMessage payload)
        {
            var body = Encoding.UTF8.GetString(payload.Body.ToArray());
            return JsonConvert.DeserializeObject<TResult>(body);
        }
    }
}
