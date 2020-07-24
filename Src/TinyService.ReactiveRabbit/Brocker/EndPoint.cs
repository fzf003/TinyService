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
        public EndPoint(IModel channel, string topic)
        {
            _channel = channel;
            _topic = topic;
            _channel.ExchangeDeclare(topic, ExchangeType.Fanout);
        }

        public void PushMessage(T obj)
        {
            try
            {
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
               
                _channel.BasicPublish(_topic, string.Empty, null, body);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not send message {e.Message}");
            }
        }

        public void PushError(Exception ex)
        {

        }
    }
}
