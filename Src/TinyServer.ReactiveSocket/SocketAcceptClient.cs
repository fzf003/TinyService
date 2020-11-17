using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TinyServer.ReactiveSocket
{
    public class SocketAcceptClient : IDisposable
    {
        readonly Socket _socket;

        readonly NetworkStream _networkstream;

        readonly PipeReader _pipeReader;

        readonly PipeWriter _pipeWriter;

        readonly TimeSpan heartbeatttimespan = TimeSpan.FromMilliseconds(800);

        readonly SocketServer _listenerserver;

        readonly ILogger<SocketAcceptClient> _logger;

        readonly Heartbeat _heartbeat;

        readonly string connectonId = string.Empty;

        readonly CancellationTokenSource cancellation = new CancellationTokenSource();
        public SocketAcceptClient(Socket socket, SocketServer listenerserver, ILoggerFactory loggerFactory,bool heartbeat=false)
        {
            this._logger = loggerFactory.CreateLogger<SocketAcceptClient>();

            this._socket = socket;
 
            this._heartbeat = new Heartbeat(_socket, loggerFactory);

            this._networkstream = new NetworkStream(_socket);

            this._pipeReader = PipeReader.Create(_networkstream);

            this._pipeWriter = PipeWriter.Create(_networkstream);
 
            this._listenerserver = listenerserver;

            this._heartbeat.OnDisconnect += HeartbeatProcess;
 
            connectonId = $"{this._socket.RemoteEndPoint}";

        }

        public string RemoteConnectionId
        {
            get
            {
                return connectonId;
            }
        }

        void HeartbeatProcess(HeartbeatEvent @event)
        {

            this._listenerserver.RemoveConnection(this);

            _logger.LogDebug($"{DateTime.Now}----{_socket.RemoteEndPoint}连接断开....");

            this.Dispose();
        }


        public IObservable<ReadOnlySequence<byte>> RevicedObservable =>
               Observable.Create<ReadOnlySequence<byte>>((observer) => ReaderSchedule(observer, cancellation.Token));


        IDisposable ReaderSchedule(IObserver<ReadOnlySequence<byte>> observer, CancellationToken cancellationToken = default)
        {
            return NewThreadScheduler.Default.Schedule(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {

                        var readresult = await _pipeReader.ReadAsync(cancellationToken).ConfigureAwait(false);

                        var buffer = readresult.Buffer;

                        if (buffer.Length <= 0)
                        {
                            continue;
                        }

                        while (ContainsLine(ref buffer, out ReadOnlySequence<byte> line))
                        {
                            ProcessLine(line, observer);

                        }
                        ////将指针移到下一条数据的头位置。
                        _pipeReader.AdvanceTo(buffer.Start, buffer.End);


                        if (readresult.IsCompleted)
                        {
                            break;
                        }

                    }
                    catch 
                    {
                       break;
                    }
                }

                _pipeReader?.Complete();
                observer.OnCompleted();
            });
        }

        // public PipeReader Reader => this._pipeReader;

         public PipeWriter Writer => this._pipeWriter;

        

        public void Dispose()
        {
            cancellation?.Cancel();
            this._heartbeat?.Dispose();
            this._pipeReader?.Complete();
            this._pipeWriter?.Complete();
            ShutDownSocket(_socket);
        }

        void ShutDownSocket(Socket socket)
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close(1000);
            _logger.LogInformation("连接关闭完成....");
        }

        static bool ContainsLine(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> line)
        {
            SequencePosition? position = buffer.PositionOf((byte)'\n');

            if (position == null)
            {
                line = default;
                return false;
            }


            line = buffer.Slice(0, position.Value);
            buffer = buffer.Slice(buffer.GetPosition(1, position.Value));
            return true;
        }


        static void ProcessLine(in ReadOnlySequence<byte> buffer, IObserver<ReadOnlySequence<byte>> observer)
        {
            observer.OnNext(buffer);
        }
    }


}
