using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using TinyService.Core;

namespace TinyService.RequestResponse
{
  
  public class Request : IRequest
    {
        public Request(object body, PID sender,bool oneway=true)
            : this(Guid.NewGuid().ToString("N"), body, sender,oneway)
        {

        }

        public Request(object body)
            : this(Guid.NewGuid().ToString("N"), body, null)
        {

        }

        public Request(string requestId, object body, PID sender,bool onway=true)
        {
            this.RequestId = requestId;

            this.Sender = sender;

            this.Body = body;

            this.OneWay = onway;
        }

        public string RequestId
        {
            get;
            private set;
        }


        public PID Sender
        {
            get;
            private set;
        }

        public PID Self
        {
            get;
            private set;
        }


      

        public void SetReplay(PID replay)
        {
            this.Sender = replay;
        }

        public void SetOneWay(bool oneway)
        {
            this.OneWay = oneway;
        }


        public object Body
        {
            get;
            private set;
        }


        public bool OneWay
        {
            get;
            private set;
        }


        public void SetSelf(PID self)
        {
            this.Self = self;
        }
    }

    public class Request<T> : Request
    {
        public Request(T body) :
            base(Guid.NewGuid().ToString("N"), body, null)
        {

        }

        public Request(object body, PID sender)
            : base(Guid.NewGuid().ToString("N"), body, sender)
        {
        }

        public T Data { get { return (T)Body; } }

        public static Request<T> Create(T message)
        {
            return new Request<T>(message);
        }
    }


    public static class RequestExt
    {
        public static Request<T> ToRequest<T>(this T messagebody)
        {
            return new Request<T>(messagebody);
        }

        public static ResponseMessage<T> ToResponse<T>(this T responsebody)
        {
            return new ResponseMessage<T>(responsebody);
        }
    }

    public class ResponseMessage<T> : ResponseMessage
    {
        public ResponseMessage(T responseBody)
            : base(responseBody)
        {

        }
    }

    public class ResponseMessage
    {
        public ResponseMessage(object result)
        {
            this.ResponseBody = result;
        }

        public object ResponseBody { get; private set; }

    }


    /// <summary>
    /// 发送消息封装
    /// </summary>
    internal class MessageWrapper 
    {
        public Dictionary<string, string> Headers { get; private set; }

         public object Body { get; private set; }

         public PID Self { get; private set; }

         public PID Sender { get; private set; }

         public MessageWrapper(Dictionary<string, string> headers, object body, PID self, PID sender = null) 
         {

             this.Body = body;

             this.Headers = headers;

             this.Self = self;

             this.Sender = sender;
         }
     }

    
}
