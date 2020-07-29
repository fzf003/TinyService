using System;
using System.Collections.Generic;
using System.Text;

namespace TinyService.Discovery.Consul
{
    public class ServiceInformation
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }
        public string Version { get; set; }
        public IEnumerable<string> Tags { get; set; }

        public Uri ToUri(string scheme = "http", string path = "/")
        {
            var builder = new UriBuilder(scheme, Address, Port, path);
            return builder.Uri;
        }

        public override string ToString()
        {
            return $"{Address}:{Port}";
        }
    }
}
