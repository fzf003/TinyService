using System;
using System.Collections.Generic;
using System.Text;

namespace Sample_2.Messages
{
   public class ResponseMessage
    {
        public string Body { get; set; }

        public ResponseMessage(string message)
        {
            this.Body = message;
        }
    }
}
