using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TinyService.ReactiveRabbit.Brocker
{
    internal abstract class RemoteProcedureBase : IDisposable
    {
        protected readonly IModel Channel;
        private readonly string _queueName;
        private readonly string _consumerTag;
        readonly ILogger<RemoteProcedureBase> _logger;
        readonly ILoggerFactory _loggerFactory;
        public string QueueName
        {
            get
            {
                return _queueName;
            }
        }

        public string ConsumerTag
        {
            get
            {
                return _consumerTag;
            }
        }

        protected RemoteProcedureBase(IModel channel, string queueName, string exchangeName, string routingKey = "", bool durable = true, ILoggerFactory loggerFactory = null)
        {
            this._loggerFactory = loggerFactory;

            this._logger = loggerFactory.CreateLogger<RemoteProcedureBase>();

            Channel = channel;

            _queueName = queueName;

            Channel.QueueDeclare(
                queue: _queueName,
                durable: durable,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            Channel.BasicQos(0, 1, false);

            if (!string.IsNullOrWhiteSpace(exchangeName))
            {
                Channel.QueueBind(queue: _queueName, exchange: exchangeName, routingKey: routingKey, null);
            }

            var consumer = new EventingBasicConsumer(Channel);

            consumer.Received += MessageReceived;

            _consumerTag = Channel.BasicConsume(_queueName, false, consumer);
        }

        protected abstract Task HandleMessage(RequestContext requestContext, IBasicProperties replyProperties);

        private async void MessageReceived(object sender, BasicDeliverEventArgs args)
        {
            try
            {
                var requestContext = DeserializeMessage(args.Body, args.BasicProperties);
                await HandleMessage(requestContext, args.BasicProperties);
                Channel.BasicAck(args.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                
                this._logger.LogError("Error:{0}", ex);

                //Channel.BasicNack(args.DeliveryTag, false, requeue: false);
            }
        }

        private static RequestContext DeserializeMessage(ReadOnlyMemory<byte> body, IBasicProperties props)
        {

            var message = new PayloadMessage
            {
                Body = body
            };

            var userContext = new RequestContext(message, props.UserId, props.AppId, props.ContentType, props.Type, props.CorrelationId,props.ReplyTo);

            return userContext;
        }

        public void Dispose()
        {
            if (Channel.IsOpen)
            {
                Channel.BasicCancel(_consumerTag);
            }
        }
    }

   

   
}
