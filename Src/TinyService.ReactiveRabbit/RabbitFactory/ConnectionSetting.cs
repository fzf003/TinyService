using System;
using System.Collections.Generic;
using System.Text;

namespace TinyService.ReactiveRabbit.RabbitFactory
{
    public sealed class ConnectionSetting
    {

        public string HostName { get; set; }


        public string VirtualHost { get; set; }


        public string UserName { get; set; }


        public string Password { get; set; }


        public int Port { get; set; }

        public ConnectionSetting()
        {
            Port = 5672;
        }
    }
}
