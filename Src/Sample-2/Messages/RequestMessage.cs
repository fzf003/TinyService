namespace Sample_2.Messages
{
    public class RequestMessage
    {
        public string Payload { get;  set; }

        public RequestMessage(string message)
        {
            this.Payload = message;
        }
    }
}