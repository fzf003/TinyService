using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace TinyService.ReactiveRabbit.Brocker
{
    public class RequestContext
    {
 

        public RequestContext(PayloadMessage requestMessage, string username, string appId, string contentType, string type, string correlationId, string replyTo)
        {
            RequestMessage = requestMessage;

            Username = username;

            AppId = appId;

            ContentType = contentType;

            Type = type;

            CorrelationId = correlationId;

            ReplyTo = replyTo;
        }

        public string ReplyTo { get; set; }

        public PayloadMessage RequestMessage { get; }

        public string Username { get; }

        public string AppId { get; set; }

        public string ContentType { get; set; }

        public string Type { get; set; }

        public string CorrelationId { get; set; }
    }
}
