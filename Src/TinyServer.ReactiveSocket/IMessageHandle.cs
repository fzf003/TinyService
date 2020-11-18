using System;
using System.Collections.Generic;
using System.Text;

namespace TinyServer.ReactiveSocket
{
    public interface IMessageHandle<T>
    {
        void OnMessage(T message);

        void OnError(Exception ex);

        void OnCompleted();
    }
}
