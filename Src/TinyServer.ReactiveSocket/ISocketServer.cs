using System;
using System.Net;

namespace TinyServer.ReactiveSocket
{
    public interface ISocketServer:IDisposable
    {
        IObservable<ISocketAcceptClient> AcceptClientObservable { get; }
        
        void Start();
    }
}