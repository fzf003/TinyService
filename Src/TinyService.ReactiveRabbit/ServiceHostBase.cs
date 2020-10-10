using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using TinyService.ReactiveRabbit.Brocker;

namespace TinyService.ReactiveRabbit
{
    public abstract class ServiceHostBase : IDisposable
    {
        private readonly IMessageBroker _broker;
        private readonly CompositeDisposable _registedCalls = new CompositeDisposable();
        public readonly string InstanceId;

        public readonly string ServiceType;

        readonly ILogger<ServiceHostBase> logger;

        protected ServiceHostBase(IMessageBroker broker,ILogger<ServiceHostBase> logger)
        {
            this.logger = logger;

            _broker = broker;

            InstanceId = nameof(ServiceHostBase) + "." + Guid.NewGuid().ToString().Substring(0, 6);
           
        }

        public virtual void Dispose()
        {
            _registedCalls.Dispose();
            
        }

        protected void RegisterCall(string exchangeName = "", string queueName = "", string routingKey = "", Func<RequestContext, IBasicProperties, Task> onMessage=null)
        {
            var call = _broker.RegisterHandle(exchangeName:exchangeName,queueName: queueName,routingKey:routingKey, onMessage: onMessage);

            _registedCalls.Add(Disposable.Create(() =>
            {
                call.Dispose();
                
            }));


          
        }

        protected void RegisterCallResponse<T>(string exchangeName = "", string queueName = "", string routingKey = "", Func<RequestContext, Task<T>> onMessage=null)
        {
            
            var call = _broker.RegisterCallResponse<T>(exchangeName: exchangeName, queueName: queueName, routingKey: routingKey, onMessage: onMessage);
            _registedCalls.Add(Disposable.Create(() =>
            {
               
                call.Dispose();
                
            }));
            
        }

       

  

        public override string ToString()
        {
            return $"{InstanceId}";
        }
    }
}
