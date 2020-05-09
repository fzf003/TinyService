using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyService.Utils;
using System.Threading;
using System.Threading.Tasks;
using TinyService.RequestResponse;

namespace TinyService.Core
{
    public class PID
    {
        public string Id { get; set; }

        public string Address { get; set; }

        private Process _process;

        public PID(string address = null, string id = null)
        {
            Address = address;
            Id = id;
        }

        internal PID(string address, string id, Process process)
            : this(address, id)
        {
            _process = process;
        }

        public Process Ref
        {
            get
            {
                var p = _process;
                if (p != null)
                {
                    p.Match()
                     .With<LocalProcess>(lp =>
                     {
                         if (lp.IsDead)
                         {
                             _process = null;
                         }
                     });

                    return _process;
                }

                var reff = ProcessRegistry.Instance.Get(this);

                if (!(reff is DeadLetterProcess))
                {
                    _process = reff;
                }

                return _process;
            }
        }

        public void Tell(object message,Dictionary<string,string> hander=null)
        {
            var reff = Ref ?? ProcessRegistry.Instance.Get(this);
            reff.SendUserMessage(this, new MessageWrapper(hander, message, this));
        }

        public void SendSystemMessage(object sys)
        {
            var reff = Ref ?? ProcessRegistry.Instance.Get(this);
          
            reff.SendSystemMessage(this, sys);
        }

        public void Request(object message, PID sender,bool Oneway=true)
        {
            var reff = Ref ?? ProcessRegistry.Instance.Get(this);
            object messageEnvelope = new RequestEvlopMessage(this,sender, message,Oneway);
            reff.SendUserMessage(this, messageEnvelope);
        }

        public void Ask(object message, PID sender,Dictionary<string,string> hander=null)
        {
            var reff = Ref ?? ProcessRegistry.Instance.Get(this);
            object messageEnvelope = new MessageWrapper(hander, message, this, sender);
            reff.SendUserMessage(this, messageEnvelope);
        }

        private Task<T> Ask<T>(object message, ReseponseFutureProcess<T> future, Dictionary<string, string> hander = null)
        {
            Ask(message, future.Pid,hander:hander);
            return future.Task;
        }

        public Task<T> Ask<T>(object message, TimeSpan? timeout = null, Dictionary<string, string> hander = null)
        {
             return Ask<T>(message, new ReseponseFutureProcess<T>(CancellationToken.None, timeout == null ? TimeSpan.FromMilliseconds(5000) : timeout.Value),hander:hander);
        }



        public Task<T> RequestAsync<T>(object message, double timeout)
        {
            return RequestAsync(message, new ReseponseFutureProcess<T>(new CancellationToken(), timeout));
        }

        public Task<T> RequestAsync<T>(object message, CancellationToken cancellationToken)
        {
            return RequestAsync(message, new ReseponseFutureProcess<T>(cancellationToken));
        }

        public Task<T> RequestAsync<T>(object message)
        {
            return RequestAsync(message, new ReseponseFutureProcess<T>());
        }

        private Task<T> RequestAsync<T>(object message, ReseponseFutureProcess<T> future)
        {
            Request( message, future.Pid,false);
            return future.Task;
        }

        public void Stop()
        {
            var reff = ProcessRegistry.Instance.Get(this);
            reff.Stop(this);
        }

        public string ToShortString()
        {
            return Address + "/" + Id;
        }
    }

}
