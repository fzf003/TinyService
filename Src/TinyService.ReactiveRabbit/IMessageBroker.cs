using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TinyService.ReactiveRabbit.Brocker;

namespace TinyService.ReactiveRabbit
{
    public interface IMessageBroker:IDisposable
    {
        IDisposable RegisterHandle(string exchangeName = "", string queueName = "", string routingKey = "", Func<RequestContext, IBasicProperties, Task> onMessage = null, IDictionary<string, object> arguments = null);
        IDisposable RegisterCallResponse<TResponse>(string exchangeName = "", string queueName = "", string routingKey = "", Func<RequestContext, Task<TResponse>> onMessage = null);
        IEndPoint<T> GetServiceEndPoint<T>(string topicName, string topicType = "direct", string routingKey = "", bool durable = true);

        IObservable<T> SubscribeToTopic<T>(string topic, string routingKey = "");
    }
}
