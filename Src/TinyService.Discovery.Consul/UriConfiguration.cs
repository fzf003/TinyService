using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TinyService.Discovery.Consul
{
    public static class UriConfiguration
    {
        public static Uri GetUri(int port = 0)
        {
            port = port == 0 ? FreeTcpPort() : port;
            var uri = new Uri($"http://localhost:{port}");
            return uri;
        }


        private static int FreeTcpPort()
        {
            var l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            var port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }
    }
}
