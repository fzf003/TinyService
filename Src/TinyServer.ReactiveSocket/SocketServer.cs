using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Extensions.Logging;

namespace TinyServer.ReactiveSocket
{
    public class SocketServer : ISocketServer
    {
        readonly Socket _serversocket;

        readonly Subject<ISocketAcceptClient> socketobserver = new Subject<ISocketAcceptClient>();

        readonly CompositeDisposable socketDisposable;

        readonly List<ISocketAcceptClient> connections = new List<ISocketAcceptClient>();

        readonly ILogger<SocketServer> _logger;

        readonly ILoggerFactory _loggerfactory;

        readonly IPEndPoint _endPoint;

        public static ISocketServer CreateServer(IPEndPoint endPoint, ILoggerFactory loggerfactory)
        {
            return new SocketServer(endPoint, loggerfactory);
        }


        public SocketServer(IPEndPoint endPoint, ILoggerFactory loggerfactory)
        {

            this.socketDisposable = new CompositeDisposable();

            this._loggerfactory = loggerfactory;

            this._logger = loggerfactory.CreateLogger<SocketServer>();

            _serversocket = new Socket(SocketType.Stream, ProtocolType.Tcp)
            {
                NoDelay = true
            };

            this._endPoint = endPoint;

        }

        public IObservable<ISocketAcceptClient> AcceptClientObservable => socketobserver.AsObservable();

        public void Start()
        {
            _serversocket.Bind(_endPoint);

            _serversocket.Listen(120);

            var disp = Observable.Defer(() => Observable.FromAsync(() => _serversocket.AcceptAsync()))
                                 .Do(PrintLog, PrintError)
                                 .Repeat()
                                 .Subscribe(ProcessLinesAsync, PrintError, ()=> {
                                     Console.WriteLine("Server Socket Done...");
                                 });

            socketDisposable.Add(disp);

            _logger.LogInformation($"服务已启动，监听端口: {_endPoint.Port}");
        }



        void PrintLog(Socket socket)
        {
            Console.ForegroundColor = ConsoleColor.Green;

            _logger.LogInformation($"远程连接:{socket.RemoteEndPoint}已接入->本地:{socket.LocalEndPoint}");

            Console.ResetColor();
        }

        void PrintError(Exception exception)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            _logger.LogError(exception.Message);

            Console.ResetColor();
        }

        public void ProcessLinesAsync(Socket socket)
        {
            var acceptclient = new SocketAcceptClient(socket, this, _loggerfactory);

            socketobserver.OnNext(acceptclient);

            connections.Add(acceptclient);

            _logger.LogInformation($"客户端:{socket.RemoteEndPoint}已连接,当前共有{connections.Count}个客户端!");
        }

        internal void RemoveConnection(ISocketAcceptClient socketclient)
        {
            this.connections.Remove(socketclient);
            socketclient?.Dispose();
            _logger.LogInformation($"客户端:{socketclient.RemoteConnectionId}已断开,当前在线:{connections.Count}个客户端");
        }


        public void Dispose()
        {
            this.socketDisposable?.Dispose();
            if (this._serversocket != null)
            {
                Helper.IgnoreException(() =>
                {
                    this._serversocket?.Shutdown(SocketShutdown.Both);
                    this._serversocket?.Close(1000);

                    connections.ForEach(socketclient =>
                    {
                        socketclient?.Dispose();
                        _logger.LogInformation($"客户端:{socketclient.RemoteConnectionId}已下线");
                    });
                });
            }
 
            _logger.LogInformation("服务端已关闭....");
        }

    }
}
