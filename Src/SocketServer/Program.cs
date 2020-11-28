using Microsoft.Extensions.Logging;
using System;
using TinySocketServer = TinyServer.ReactiveSocket.SocketServer;
using TinyServer.ReactiveSocket;
using System.Net;
using System.Text;
using System.Reactive.Linq;
using System.IO.Pipelines;
using System.IO;
using System.Reactive;

namespace SocketServer

{
    class Program
    {
        static ILogger<Program> logger;

        static ILoggerFactory loggerFactory;
        static void Main(string[] args)
        {
            loggerFactory = LoggerFactory.Create(options =>
            {
                options.SetMinimumLevel(LogLevel.Information)
                       .AddConsole();
            });

            logger = loggerFactory.CreateLogger<Program>();

            var serverendpoint = new IPEndPoint(IPAddress.Loopback, 8087);

            using var server = TinySocketServer.CreateServer(serverendpoint, loggerFactory);
 
            server.AcceptClientObservable.Subscribe(ReplyClient);

            server.Start();

            Console.ReadKey();
        }

        static void ReplyClient(ISocketAcceptClient acceptClient)
        {
            acceptClient.RevicedObservable
                        .Select(bytes=>bytes.ToMessage())
                        .Where(message=>!string.IsNullOrWhiteSpace(message))
                        .PrintHandler(new PrintMessageHandle(loggerFactory))
                        .Select(bytes => Observable.FromAsync(() => acceptClient.SendMessageAsync(DateTime.Now.ToString())))
                        .Concat()
                        .Catch<Unit, Exception>(ex => Observable.Empty<Unit>())
                        .Subscribe();
        }
     }
 }
