using System;
using TinyService.Utils;
using TinyService.RequestResponse;

namespace TinyService.Core
{

    public class RequestEvlopMessage
    {
        public PID Sender { get; private set; }

        public object Request { get; private set; }

        public bool OneWay { get;private set; }

        public PID Self { get; private set; }

        public RequestEvlopMessage(PID self,PID sender, object Request,bool onway=true)
        {
            this.Sender = sender;
            this.Request = Request;
            this.OneWay = onway;
            this.Self = self;
        }

        public static T Unwrap<T>(object message) where T:IRequest
        {
         
            T result=default(T);

            message.Match()
                   .With<RequestEvlopMessage>(p =>
                   {
                       var temprequest= (T)p.Request;

                       temprequest.SetOneWay(p.OneWay);

                       temprequest.SetSelf(p.Self);

                       if(p.Sender!=null)
                       {
                           temprequest.SetReplay(p.Sender);
                        }

                       result = temprequest;
                   });

            return result;
         
        }
     }


    public class RequestMessage
    {
        public RequestMessage(object result)
        {
            this.ResponseBody = result;
        }

        public object ResponseBody { get; private set; }
    }


    public class ResponseMessageEnvelope
    {
        public ResponseMessageEnvelope(object message, IActor sender)
        {
            Sender = sender;
            Message = message;
        }

        public IActor Sender { get; private set; }

        public object Message { get; private set; }

        public static Tuple<object, IActor> Unwrap(object message)
        {
            Tuple<object, IActor> result = null;

            message.Match()
                   .With<ResponseMessageEnvelope>(p =>
                   {
                       result = Tuple.Create<object, IActor>(p.Message, p.Sender);
                   });

            return result == null ? Tuple.Create<object, IActor>(message, null) : result;
        }

    }


  
}
