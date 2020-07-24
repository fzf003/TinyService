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
        private readonly Func<RequestContext, Task> _messageHandler;

        readonly ILogger<RemoteSubscriptionRegistration> _logger;

        readonly ILoggerFactory _loggerFactory;

        public RemoteSubscriptionRegistration(IModel channel, string queueName, string exchangeName = "", bool durablequeue = true, Func<RequestContext, Task> messageHandler = null, ILoggerFactory loggerFactory=null)
            : base(channel, queueName, exchangeName, string.Empty,durablequeue, loggerFactory: loggerFactory)
        {
            this._loggerFactory = loggerFactory;

            this._logger = loggerFactory.CreateLogger<RemoteSubscriptionRegistration>();

            _messageHandler = messageHandler;
        }

        protected override async Task HandleMessage(RequestContext requestContext, IBasicProperties _)
        {
            await _messageHandler(requestContext);
        }


    }
}
