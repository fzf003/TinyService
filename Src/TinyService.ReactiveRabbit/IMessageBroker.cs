using System;
using System.Threading.Tasks;
using TinyService.ReactiveRabbit.Brocker;

namespace TinyService.ReactiveRabbit
{
    public interface IMessageBroker
    {
        IDisposable RegisterHandle(string exchangeName = "", string queueName = "", string routingKey = "", Func<RequestContext, Task> onMessage = null);

        IEndPoint<T> GetServiceEndPoint<T>(string topicName, string topicType = "direct", string routingKey = "", bool durable = true);

        IObservable<T> SubscribeToTopic<T>(string topic, string routingKey = "");
    }
}
