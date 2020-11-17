using Microsoft.Extensions.Logging;
using System;
using TinySocketServer = TinyServer.ReactiveSocket.SocketServer;
using TinyServer.ReactiveSocket;
using System.Net;
using System.Text;
using System.Reactive.Linq;

namespace SocketServer

{
    class Program
    {
        static void Main(string[] args)
        {
            var loggerfactory = LoggerFactory.Create(options =>
            {
                options.SetMinimumLevel(LogLevel.Information)
                       .AddConsole();
            });

            var logger = loggerfactory.CreateLogger<Program>();

            using var server = TinySocketServer.CreateServer(new IPEndPoint(IPAddress.Loopback, 8087), loggerfactory);

            server.AcceptClientObservable.Subscribe(client =>
               {
                   client.RevicedObservable
                         .Select(bytes => bytes.ToMessage())
                         .Subscribe(async message =>
                           {
                               logger.LogInformation(message);

                               await client.Writer.SendAsync(DateTime.Now.ToString().ToMessageBuffer());
  
                           });
               });

            server.Start();


            Console.ReadKey();
        }
    }
}
