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
        static void Main(string[] args)
        {
            var loggerfactory = LoggerFactory.Create(options =>
            {
                options.SetMinimumLevel(LogLevel.Information)
                       .AddConsole();
            });

            var logger = loggerfactory.CreateLogger<Program>();

            string Replymessage = $"{DateTime.Now.ToString()}-{Guid.NewGuid().ToString("N")}";

            using (client = TinySocketClient.CreateClient(new IPEndPoint(IPAddress.Loopback, 8087), loggerfactory))
            {

                client.ReceiveMessageObservable
                    .Select(p => p.ToMessage())
                    .Do(PrintMessage)
                    .Subscribe(onNext: PrintMessage, onError: PrintError,onCompleted: Completed);

               
                client.SendMessageAsync(DateTime.Now.ToString().ToMessageBuffer());


                Console.ReadKey();
            }
        }

        static void PrintMessage(Unit message)
        {
            Console.WriteLine(message);
        }

        static void PrintMessage(string message)
        {
            Console.WriteLine(message);
            client.SendMessageAsync(message);
               
        }

        static void PrintError(Exception exception)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(exception.Message);
            Console.ResetColor();
        }

        static void Completed()
        {
            Console.WriteLine("Done...");
        }
    }
}
