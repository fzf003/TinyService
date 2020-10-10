using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TinyService.ReactiveRabbit.Brocker
{
    internal class RemoteRequestRegistration<TResponse> : RemoteProcedureBase
    {
        private readonly Func<RequestContext,  Task<TResponse>> _messageHandler;
 
        public RemoteRequestRegistration(IModel channel, string queueName, string exchangeName = "", string routingKey = "", bool durablequeue = true, Func<RequestContext, Task<TResponse>> messageHandler = null, ILoggerFactory loggerFactory = null)
            : base(channel, queueName, exchangeName, routingKey, durablequeue, loggerFactory: loggerFactory)
        {
            _messageHandler = messageHandler;
        }

        protected override async Task HandleMessage(RequestContext requestContext, IBasicProperties requestProperties)
        {

            var result = await _messageHandler(requestContext);

            var replyProperties = CreateReplyProperties(requestProperties);
            var responseBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result));
            Channel.BasicPublish(
                exchange: string.Empty,
                routingKey: requestContext.ReplyTo,
                basicProperties: replyProperties,
                body: responseBytes);
        }

        private IBasicProperties CreateReplyProperties(IBasicProperties requestProperties)
        {
            var replyProperties = Channel.CreateBasicProperties();
            replyProperties.CorrelationId = requestProperties.CorrelationId;
            return replyProperties;
        }
    }
}
