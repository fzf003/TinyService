using System;
using System.Collections.Generic;
using System.Text;

namespace TinyService.Discovery.Consul
{
    internal interface IServiceUrlProvider
    {
        Uri GetUri(string serviceName, string version);
    }


   
}
