using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TinyService.Discovery.Consul
{
    internal static class ClusterProviderExtensions
    {

        static long serviceindex = 0;

        public static async Task<ServiceInformation> FindServiceInstanceAsync(this IClusterProvider self, string serviceName)
        {
            var res = await self.FindServiceInstancesAsync(serviceName).ConfigureAwait(false);
            if (res.Length == 0)
                throw new Exception("没有发现该服务");

            var i = serviceindex % res.Count();

            var serviceinfo = res[i];

            if (i >= long.MaxValue)
            {
                i = long.MinValue;
            }

            Interlocked.Increment(ref serviceindex);

            return serviceinfo;
        }
    }
}
