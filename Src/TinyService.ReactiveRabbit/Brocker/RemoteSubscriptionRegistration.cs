using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TinyService.ReactiveRabbit.Brocker
{
    internal class RemoteSubscriptionRegistration : RemoteProcedureBase
    {
        private readonly Func<RequestContext, IBasicProperties, Task> _messageHandler;

        readonly ILogger<RemoteSubscriptionRegistration> _logger;

        readonly ILoggerFactory _loggerFactory;

        public RemoteSubscriptionRegistration(IModel channel, string queueName, string exchangeName = "", string routingKey = "", bool durablequeue = true, Func<RequestContext, IBasicProperties, Task> messageHandler = null, ILoggerFactory loggerFactory=null,IDictionary<string,object> arguments=null)
            : base(channel, queueName, exchangeName, routingKey, durablequeue, loggerFactory: loggerFactory,arguments:arguments)
        {
            this._loggerFactory = loggerFactory;

            this._logger = loggerFactory.CreateLogger<RemoteSubscriptionRegistration>();

            _messageHandler = messageHandler;
        }

        protected override async Task HandleMessage(RequestContext requestContext, IBasicProperties  properties)
        {
 
            await _messageHandler(requestContext, properties);
        }


    }
}
