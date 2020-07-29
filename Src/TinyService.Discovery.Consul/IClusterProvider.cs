using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TinyService.Discovery.Consul
{
    public interface IClusterProvider
    {
        Task<ServiceInformation[]> FindServiceInstancesAsync(string name);

        Task<ServiceInformation> RegisterServiceAsync(string serviceName, string serviceId, string version, Uri uri = null, IEnumerable<string> tags = null);

        Task BootstrapClientAsync();

        Task KvPutAsync(string key, object value);

        Task<T> KvGetAsync<T>(string key);

        Task KvDeleteAsync(string key);

        Task<bool> DeregisterServiceAsync(string serviceId);


    }

}
