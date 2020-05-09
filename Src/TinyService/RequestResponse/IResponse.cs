

using TinyService.Core;

namespace TinyService.RequestResponse
{
    public interface IResponse : IMessage
    {

    }
    public interface IMessage
    {

    }

    public interface IRequest : IMessage
    {
        string RequestId { get; }

        PID Sender { get; }

        PID Self { get; }

        bool OneWay { get; }

        void SetReplay(PID replay);

        void SetSelf(PID self);

        void SetOneWay(bool oneway);

        object Body { get; }
     }

}