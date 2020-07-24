using System;
using System.Collections.Generic;
using System.Text;

namespace TinyService.ReactiveRabbit
{
    public interface IEndPoint<T>
    {
        void PushMessage(T message);
        void PushError(Exception ex);
    }
}
