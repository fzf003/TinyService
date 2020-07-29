using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TinyService.Discovery.Consul
{
    public interface IClusterClinet
    {
        Task<ServiceInformation[]> FindServiceInstancesAsync(string servicename);

        Task<ServiceInformation> FindServiceInstanceAsync(string servicename);

        Task KvPutAsync(string key, object value);

        Task<T> KvGetAsync<T>(string key);

        Task DeregisterServiceAsync(string serviceId);


        Task<ServiceInformation> RegisterServiceAsync(string serviceName, string serviceId, string version, Uri uri = null, IEnumerable<string> tags = null);


    }

   
}
