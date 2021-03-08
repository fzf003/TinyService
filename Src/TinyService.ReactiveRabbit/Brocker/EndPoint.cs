using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace TinyService.ReactiveRabbit.Brocker
{
    internal class EndPoint<T> : IEndPoint<T>
    {
        private readonly IModel _channel;

        private readonly string _topic;

        readonly string _routingKey;

        public EndPoint(IModel channel, string topic, string topictype = "fanout", string routingKey = "", bool durable = true)
        {
            _channel = channel;
            _topic = topic;
            _routingKey = routingKey;
            _channel.ExchangeDeclare(topic, topictype, durable: durable);
        }

        public void PushMessage(T obj)
        {
                 var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
                 var props= _channel.CreateBasicProperties();
                 _channel.BasicPublish(exchange: _topic, routingKey: _routingKey, props, body);
        }

        public void PushError(Exception ex)
        {

        }
    }
}
