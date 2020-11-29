using Microsoft.Extensions.Logging;
using System;
using TinySocketClient = TinyServer.ReactiveSocket.SocketClient;
using System.Net;
using System.Reactive.Linq;
using TinyServer.ReactiveSocket;
using System.Reactive;
using System.Threading.Tasks;

namespace SocketClient
{
    class Program
    {
        static ISocketClient client;
        static ILoggerFactory loggerFactory;
        static void Main(string[] args)
        {
            loggerFactory = LoggerFactory.Create(options =>
            {
                options.SetMinimumLevel(LogLevel.Information)
                       .AddConsole();
            });

            var logger = loggerFactory.CreateLogger<Program>();

            string Replymessage = $"{DateTime.Now.ToString()}-{Guid.NewGuid().ToString("N")}";

            using (client = TinySocketClient.CreateClient(new IPEndPoint(IPAddress.Loopback, 8087), loggerFactory))
            {

                client.ReceiveMessageObservable
                      .Select(p => p.ToMessage())
                      .Subscribe(new PrintMessageHandle(loggerFactory));
 

                client.SendMessageAsync($"{Guid.NewGuid().ToString("N")}");

                Console.ReadKey();

            }
        }

    }
}
