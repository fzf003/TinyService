using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks.Dataflow;

using System.Threading;
using System.Threading.Tasks;
using TinyService.Utils;
using TinyService.RequestResponse;
using TinyService.ServiceBus;

namespace TinyService.Core
{
    public abstract class ProcessActor<TState> : ProcessActor where TState : class,new()
    {
        public ProcessActor()
        {
            this.State = new TState();
        }

        public TState State { get;  set; }

        protected virtual Task WriteStateAsync()
        {
             return Task.FromResult(0);
        }


    }




    public class ProcessActor : Process
    {
        public ProcessActor()
            : base(new CancellationTokenSource(), string.Empty)
        {


        }

        public ProcessActor(string id)
            : base(new CancellationTokenSource(), id)
        {


        }
        protected internal override void SendSystemMessage(PID pid, object message)
        {

        }
        protected internal override void SendUserMessage(PID pid, object message)
        {
            var isresponse = message
                             .Match()
                             .With<RequestEvlopMessage>(p => //解析请求/响应消息
                             {
                                 var request = RequestEvlopMessage.Unwrap<IRequest>(p);

                                 this.SendAsync(request);

                             })
                             .With<MessageWrapper>(p =>
                             {

                                 this.SendAsync(message);

                             }).Default(msg => {
                                 EventBus.Instance.Publish(new DeadLetterEvent(pid, message, null));
                             }).WasHandled;

          
        }

        public override string ToString()
        {
            return string.Format("{0}", this.GetType().FullName);
        }
    }
}
