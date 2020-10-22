using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Sample_2.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TinyService.ReactiveRabbit;
using TinyService.ReactiveRabbit.Brocker;

namespace Sample_2.Services
{

    public interface IServiceHost
    {
        void Start();

        IEndPoint<RequestMessage> GetRequestEndPoint { get; }
    }


    public class RequestProcessServiceHost: ServiceHostBase,IServiceHost
    {
        readonly IMessageBroker _messageBroker;

        readonly ILogger<RequestProcessServiceHost> _logger;

        readonly IEndPoint<RequestMessage> _rquestEndPoint;
  
        const string exchangename = "PayService.RequestTopic";

        const string ququename = "PayService.ResponseConsumer";
 
        public RequestProcessServiceHost(IMessageBroker messageBroker, ILogger<RequestProcessServiceHost> logger)
            :base(messageBroker, logger)
        {
            _messageBroker = messageBroker;

            _logger = logger;

            this._rquestEndPoint= _messageBroker.GetServiceEndPoint<RequestMessage>(topicName: exchangename, topicType: ExchangeType.Direct, routingKey: "logs");
 
        }

        public void Start()
        {
            Console.WriteLine($"{InstanceId}启动.....");
        
            ///注册请求回复处理器
            RegisterCallResponse(exchangeName: exchangename, queueName: ququename,routingKey: "logs", onMessage: RequestProcess);

            ///接受回复消息
            RegisterCall(exchangeName: exchangename,queueName: "PayService.fzf003", routingKey: "logs", onMessage: CallbackResponseHandle);

            ///单向处理
            // RegisterCall(queueName: ququename, onMessage: ProcesseHandle);
            ///监听Topic消息
            /// _messageBroker.SubscribeToTopic<RequestMessage>(exchangename,"logs").Subscribe(p=>Console.WriteLine("Payload:"+p.Payload));
        }

        private   Task ProcesseHandle(RequestContext context, IBasicProperties properties)
        {
            var request = GetJsonPayload<RequestMessage>(context.RequestMessage);

            Console.WriteLine($"{ququename}:{request.Payload}");
  
            return Task.CompletedTask;
        }

 
        /// <summary>
        /// 处理请求处理
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private  async Task<ResponseMessage> RequestProcess(RequestContext context)
        {
            context.ReplyTo = "PayService.fzf003";//设置回复地址
           
            var request = GetJsonPayload<RequestMessage>(context.RequestMessage);

            Console.WriteLine($"fzf003:{request}");

            _logger.LogInformation($"正在处理请求,{request.Payload},回复地址:{context.ReplyTo}");

            return await new ValueTask<ResponseMessage>(new ResponseMessage($"Response:{request.Payload}"));
        }

        private  Task CallbackResponseHandle(RequestContext context, IBasicProperties properties)
        {
            var request = GetJsonPayload<ResponseMessage>(context.RequestMessage);

             _logger.LogInformation($"PayService.fzf003 接收到回复消息:{request.Body}");

  
            return Task.CompletedTask;
        }


        private static TResult GetJsonPayload<TResult>(PayloadMessage payload)
        {
            var body = Encoding.UTF8.GetString(payload.Body.ToArray());
            return JsonConvert.DeserializeObject<TResult>(body);
        }
        /// <summary>
        ///////接受请求断点
        /// </summary>
        public IEndPoint<RequestMessage> GetRequestEndPoint
        {
            get
            {
                return this._rquestEndPoint;
            }
        }
    }
}
