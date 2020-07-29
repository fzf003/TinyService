using System;

namespace TinyService.Discovery.Consul
{
    public class ServiceConfig
    {
        public ServiceConfig()
        {
            this.serviceId = Guid.NewGuid().ToString("N");
        }

        public string serviceName { get; set; }

        public string serviceId { get; set; }

        public Uri serviceUri { get; set; }

        public string version { get; set; }

        public string[] Tags { get; set; }

    }
}
