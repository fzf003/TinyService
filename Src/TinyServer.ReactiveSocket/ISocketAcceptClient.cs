using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading.Tasks;

namespace TinyServer.ReactiveSocket
{
    public interface ISocketAcceptClient:IDisposable
    {
        string RemoteConnectionId { get; }
        IObservable<ReadOnlySequence<byte>> RevicedObservable { get; }

        Task SendMessageAsync(byte[] message);
        Task SendMessageAsync(string message);
    }
}