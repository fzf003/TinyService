using RabbitMQ.Client;

namespace TinyService.ReactiveRabbit.RabbitFactory
{
    public interface IChannelFactory
    {
        void CloseConnection();
        IModel Create(bool automaticRecoveryEnabled = true, ushort requestedHeartbeat = 0, uint requestedFrameMax = 0, ushort requestedChannelMax = 0, bool useBackgroundThreadsForIo = true);
        IConnection CreateConnection(bool automaticRecoveryEnabled = true, ushort requestedHeartbeat = 0, uint requestedFrameMax = 0, ushort requestedChannelMax = 0, bool useBackgroundThreadsForIo = true);
    }
}