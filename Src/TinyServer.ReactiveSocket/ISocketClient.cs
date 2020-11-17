

using System;
using System.Buffers;
using System.Threading.Tasks;

namespace TinyServer.ReactiveSocket
{
    public interface ISocketClient : IDisposable
    {
        Task SendMessageAsync(byte[] message);
        Task SendMessageAsync(string message);
        IObservable<ReadOnlySequence<byte>> ReceiveMessageObservable { get; }
     }
}