using System;
using System.Collections.Generic;
using System.Text;

namespace TinyService.Discovery.Consul.Internal
{
    internal class ServiceUrlProvider : IServiceUrlProvider
    {
        public Uri GetUri(string serviceName, string version)
        {
            return UriConfiguration.GetUri();
        }
    }
}
