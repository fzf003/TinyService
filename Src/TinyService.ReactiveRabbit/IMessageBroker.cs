using System;
using System.Threading.Tasks;
using TinyService.ReactiveRabbit.Brocker;

namespace TinyService.ReactiveRabbit
{
    public interface IMessageBroker
    {
        IDisposable RegisterHandle(string exchangeName = "", string queueName = "", Func<RequestContext, Task> onMessage = null);


        IEndPoint<T> GetServicePublicEndPoint<T>(string queuename);

        IObservable<T> SubscribeToTopic<T>(string topic);
    }
}
