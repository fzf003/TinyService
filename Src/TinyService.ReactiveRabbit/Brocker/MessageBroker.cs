﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyService.ReactiveRabbit.Brocker
{
    public class MessageBroker : IMessageBroker
    {
        private readonly IModel _channel;

        readonly ILoggerFactory _loggerFactory;

        readonly ILogger<MessageBroker> _logger;
        public MessageBroker(IModel channel,ILoggerFactory loggerFactory)
        {
            _channel = channel;
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<MessageBroker>();
        }

        public IDisposable RegisterHandle(string exchangeName = "", string queueName = "", Func<RequestContext, Task> onMessage = null)
        {
            return new RemoteSubscriptionRegistration(_channel,  queueName: queueName, exchangeName: exchangeName,  messageHandler: onMessage,loggerFactory: _loggerFactory);
        }
 

        public IEndPoint<T> GetServicePublicEndPoint<T>(string topicName)
        {
            return new EndPoint<T>(_channel, topicName);
        }

        public IObservable<T> SubscribeToTopic<T>(string topic)
        {
            _channel.ExchangeDeclare(topic, ExchangeType.Fanout);
            var queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: queueName, exchange: topic, routingKey: string.Empty);

            var consumer = new EventingBasicConsumer(_channel);
            var observable = Observable.FromEventPattern<BasicDeliverEventArgs>(
                    x => consumer.Received += x,
                    x => consumer.Received -= x)
                .Select(GetJsonPayload<T>);
            _channel.BasicConsume(queueName, true, consumer);

            return observable;
        }

        private static TResult GetJsonPayload<TResult>(EventPattern<BasicDeliverEventArgs> arg)
        {
            var body = Encoding.UTF8.GetString(arg.EventArgs.Body.ToArray());
            return JsonConvert.DeserializeObject<TResult>(body);
        }
    }
}
