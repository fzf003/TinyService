using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

using TinyService.Utils;
using System.Threading;
using TinyService.RequestResponse;
using TinyService.ServiceBus;

namespace TinyService.Core
{
    public interface IActor : IDisposable
    {
        Task<bool> SendAsync(object message);
    }


    public class ActorContext : IDisposable
    {
        public object Message { get; private set; }

        public PID Sender { get; private set; }

        public bool OneWay { get; private set; }

        public PID Self { get; private set; }
 
        public void Respond(object message)
        {
            if (OneWay)
            {
                Sender.Tell(message);
            }
            else
            {
                Sender.Tell(message.ToResponse());
            }
        }

        public ActorContext(object message)
        {
 
            message.Match()
                   .With<IRequest>(p =>
                                   {
                                       this.Sender = p.Sender;

                                       this.OneWay = p.OneWay;

                                       this.Message = message;

                                       this.Self = p.Self;
 
                                   })
                   .With<MessageWrapper>(p =>
                   {

                       this.Self = p.Self;

                       this.Message = p.Body;

                       if (p.Sender != null)
                       {
                           this.Sender = p.Sender;
                           this.OneWay = false;
                       }
                    });
                  

        }

        public void Dispose()
        {

        }
    }



    public abstract class Actor : IActor
    {
        private readonly ActionBlock<object> _action;

        readonly IPropagatorBlock<object, object> _buffer;

        readonly CancellationTokenSource _cts;

        readonly CancellationToken token;

        private ActorContext _context;

        protected ActorContext Context { get { return _context; } }

        protected PID Self { get; private set; }

        protected PID Sender { get; private set; }

        public CancellationTokenSource CancellationTokenSource
        {
            get
            {
                return _cts;
            }
        }

        public Task Completion
        {
            get { return _buffer.Completion; }
        }

        protected virtual void Complete()
        {
          
            this._buffer.Complete();
        }


        public Actor()
            : this(new BufferBlock<object>(), new CancellationTokenSource())
        {

        }

        public Actor(BufferBlock<object> buffer, CancellationTokenSource cts)
        {
            _cts = cts;

            token = _cts.Token;

            var config = new ExecutionDataflowBlockOptions
            {
                CancellationToken = token,
                SingleProducerConstrained = true
            };

            this._buffer = buffer;

            _action = new ActionBlock<object>(message => Process(message), config);

            this._buffer.LinkTo(_action, new DataflowLinkOptions() { PropagateCompletion = true });

            this._buffer.Completion.ContinueWith(t =>
            {
                while (_action.InputCount > 0)
                {
                    Thread.Sleep(1000);
                }
                _action.Complete();

                _action.Completion.GetAwaiter().GetResult();
                 
            });

        }

        void Process(object message)
        {
            _context = new ActorContext(message);
            {
                dynamic self = this;

                dynamic msg = _context.Message;

                if (this._context.Self != null)
                {

                    this.Self = _context.Self;
                    
                }

                if (this._context.Sender != null)
                {
                    this.Sender = _context.Sender;
                }

  
                try
                {
  
                    self.Handle(msg);
                }
                catch (Exception ex)
                {
                   self.Handle(new ActorErrorMessage() { Exception = ex, Message = msg });
                   
                }
                finally
                {

                }
            }
        }


        public virtual void Handle(ActorErrorMessage message)
        {
            EventBus.Instance.Publish(message);
        }

        public virtual void Handle(Started message)
        {

        }

        public Task<bool> SendAsync(object message)
        {
            return this._buffer.SendAsync(message);
        }


        public void Dispose()
        {

            this.Complete();
            /*
            if (_cts != null)
            {
                
                _cts.Cancel();
              
             }*/
        }
    }

    public class ActorErrorMessage
    {
        public object Message
        {
            get;
            set;
        }

        public Exception Exception { get; set; }
    }







}
