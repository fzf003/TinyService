using System;
using System.Net;

namespace TinyServer.ReactiveSocket
{
    public interface ISocketServer:IDisposable
    {
        IObservable<SocketAcceptClient> AcceptClientObservable { get; }
        
        void Start();
    }
}