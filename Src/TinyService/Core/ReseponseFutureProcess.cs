using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TinyService.Utils;
using TinyService.RequestResponse;

namespace TinyService.Core
{
    public class ReseponseFutureProcess<T> : ProcessActor
    {
        private readonly CancellationTokenSource _cts;

        private readonly TaskCompletionSource<T> _tcs;

        internal ReseponseFutureProcess(CancellationToken cancellationToken, TimeSpan timeout) : this(cancellationToken, timeout.TotalMilliseconds) { }

        internal ReseponseFutureProcess(CancellationToken cancellationToken, double timeout = 5*60*60*1000) : this(CancellationTokenSource.CreateLinkedTokenSource(cancellationToken), timeout) { }

        internal ReseponseFutureProcess() : this(null, 0) { 
           
        }

        public Task<T> Task { get; private set; }

        public double OutTimeSpan { get; private set; }

        public PID Pid { get; private set; }

        readonly string _id;

        protected override string Id
        {
            get
            {
                return _id;
            }
        }
 
        ReseponseFutureProcess(CancellationTokenSource cts, double timeout)
        {
            this._id = Guid.NewGuid().ToString();
            _tcs = new TaskCompletionSource<T>();
            _cts = cts;
            this.OutTimeSpan = timeout;
            var name = this.Id; 
            var pidinfo = ProcessRegistry.Instance.TryAdd(name, this);
            Pid = pidinfo.Item1;
            
 
            if (!pidinfo.Item2)
            {
                throw new ProcessNameExistException(name);
            }

     

            if (cts != null)
            {

                TaskExt.Delay(this.OutTimeSpan, cts.Token)
                       .ContinueWith(t =>
                       {
                           if (!_tcs.Task.IsCompleted)
                           {
                               _tcs.TrySetException(new TimeoutException("请求超时!"));

                               this.Dispose();
                           }
                       });
            }

            Task = _tcs.Task;
        }


        public override void Handle(ActorErrorMessage message)
        {
            _tcs.TrySetException(message.Exception);
      
            Pid.Stop();
        }


        public void Handle(ResponseMessage message)
        {
            SendUserMessage(this, message.ResponseBody);
        }

        void SendUserMessage(IActor pid, object message)
        {
            var env = ResponseMessageEnvelope.Unwrap(message);

            if (env.Item1 is T || env.Item1 == null)
            {
                if (_cts != null && _cts.IsCancellationRequested)
                {
                    return;
                }

                _tcs.TrySetResult((T)env.Item1);

               
                Pid.Stop();
            }
            else
            {
                throw new InvalidOperationException(string.Format("Unexpected message.  Was type {0} but expected {1}", env.Item1.GetType(), typeof(T)));
            }

        }
     }
}
