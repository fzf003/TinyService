using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace TinyService.ReactiveRabbit.RabbitFactory
{
    public class ChannelFactory : IChannelFactory
    {

        private readonly object SyncObj = new object();


        private IConnection _connection;

        private ConnectionSetting _connectionSetting;

        private string _clientProvidedName;

        public ChannelFactory(ConnectionSetting connectionConfig, string clientProvidedName = null)
        {
            this._connectionSetting = connectionConfig;

            this._clientProvidedName = clientProvidedName;
        }


        public IModel Create(bool automaticRecoveryEnabled = true,
            ushort requestedHeartbeat = 0, uint requestedFrameMax = 0, ushort requestedChannelMax = 0, bool useBackgroundThreadsForIo = true)
        {
            var factory = new ConnectionFactory
            {
                HostName = this._connectionSetting.HostName,
                VirtualHost = this._connectionSetting.VirtualHost,
                UserName = this._connectionSetting.UserName,
                Password = this._connectionSetting.Password,
                Port = this._connectionSetting.Port,
                AutomaticRecoveryEnabled = automaticRecoveryEnabled,
                RequestedHeartbeat = TimeSpan.FromSeconds(requestedHeartbeat),
                RequestedFrameMax = requestedFrameMax,
                RequestedChannelMax = requestedChannelMax,
                UseBackgroundThreadsForIO = useBackgroundThreadsForIo

            };




            if (_connection == null || !_connection.IsOpen)
            {
                lock (SyncObj)
                {
                    if (_connection == null || !_connection.IsOpen)
                    {
                        _connection = factory.CreateConnection(this._clientProvidedName);
                    }
                }
            }

            var channel = _connection.CreateModel();

            return channel;
        }

        public IConnection CreateConnection(bool automaticRecoveryEnabled = true,
            ushort requestedHeartbeat = 0, uint requestedFrameMax = 0, ushort requestedChannelMax = 0, bool useBackgroundThreadsForIo = true)
        {

            var factory = new ConnectionFactory
            {
                HostName = this._connectionSetting.HostName,
                VirtualHost = this._connectionSetting.VirtualHost,
                UserName = this._connectionSetting.UserName,
                Password = this._connectionSetting.Password,
                Port = this._connectionSetting.Port,
                AutomaticRecoveryEnabled = automaticRecoveryEnabled,
                RequestedHeartbeat = TimeSpan.FromSeconds(requestedHeartbeat),
                RequestedFrameMax = requestedFrameMax,
                RequestedChannelMax = requestedChannelMax,
                UseBackgroundThreadsForIO = useBackgroundThreadsForIo,

            };

            var connection = factory.CreateConnection(this._clientProvidedName);

            return connection;
        }


        public void CloseConnection()
        {
            _connection?.Close();
            _connection?.Dispose();
            _connection = null;
        }
    }
}
