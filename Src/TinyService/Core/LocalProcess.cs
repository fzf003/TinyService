using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks.Dataflow;

namespace TinyService.Core
{

    public abstract class Process : Actor
    {
        protected internal abstract void SendUserMessage(PID pid, object message);
        private long _isDead;

        private string _id;

        protected virtual string Id
        {
            get
            {
                return this._id;
            }
        }

        public Process()
            : this(string.Empty)
        {

        }

        public Process(string id)
            : this(new CancellationTokenSource(), id)
        {

        }

        public Process(CancellationTokenSource cts, string id)
            : base(new BufferBlock<object>(), cts)
        {
            this._id = id;
        }


        internal bool IsDead
        {
            get { return Interlocked.Read(ref _isDead) == 1; }
            private set { Interlocked.Exchange(ref _isDead, value ? 1 : 0); }
        }

        public virtual void Stop(PID pid)
        {
            if (pid.Ref != null)
            {

                this.Complete();
                this.Completion.Wait();
                ProcessRegistry.Instance.Remove(pid);

                pid = null;

                IsDead = true;
            }


        }

        protected internal abstract void SendSystemMessage(PID pid, object message);
    }


    public class LocalProcess : Process
    {


        public LocalProcess()
            : base(new CancellationTokenSource(), string.Empty)
        {

        }


        protected internal override void SendUserMessage(PID pid, object message)
        {
            Console.WriteLine("用户消息:{0}--{1}", pid, message);
        }

        protected internal override void SendSystemMessage(PID pid, object message)
        {
            Console.WriteLine("系统消息:{0}--{1}", pid, message);
        }

        public override void Handle(ActorErrorMessage message)
        {
            
        }
     }

}
