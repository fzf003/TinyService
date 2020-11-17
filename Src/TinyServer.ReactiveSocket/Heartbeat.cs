
using System;
using System.Net.Sockets;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TinyServer.ReactiveSocket
{

    internal class HeartbeatEvent
    {
        public HeartbeatEvent(Socket socket, bool state)
        {
            Socket = socket;
            State = state;
        }

        public Socket Socket { get; }
        public bool State { get; }
    }



    internal class Heartbeat : IDisposable
    {
        private readonly IObservable<long> _heartbeatStream;

        private IDisposable _disp = Disposable.Empty;

        readonly ILoggerFactory _loggerFactory;

        readonly ILogger<Heartbeat> _logger;

        readonly Socket _socket;

        public event Action<HeartbeatEvent> OnDisconnect;

        public Heartbeat(Socket socket, ILoggerFactory loggerFactory)
        {
            this._socket = socket;
            HeartbeatInterval = TimeSpan.FromSeconds(5);
            _heartbeatStream = Observable.Timer(HeartbeatInterval, HeartbeatInterval);
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<Heartbeat>();
        }

        public TimeSpan HeartbeatInterval { get; }

        public void Dispose()
        {
            _disp.Dispose();
            _logger.LogDebug($"Stopped heartbeat for {_socket.RemoteEndPoint}");
        }

        public Task Start()
        {
            _disp = _heartbeatStream.Subscribe(HeartbeatProcess);

            _logger.LogDebug($"开始心跳探测 {_socket.RemoteEndPoint}");

            return Task.CompletedTask;
        }

        void HeartbeatProcess(long timer)
        {
            var socketstate = this._socket.Poll(100, SelectMode.SelectRead);
            if (socketstate)
            {
                _logger.LogDebug($"{DateTime.Now}-Endpoint:{_socket.RemoteEndPoint}掉线:{socketstate}");

                var handler = OnDisconnect;

                handler?.Invoke(new HeartbeatEvent(_socket,socketstate));
            }
            else
            {
                _logger.LogDebug($"{DateTime.Now}-Endpoint:{_socket.RemoteEndPoint}正常:{socketstate}");
            }
        }
    }
}