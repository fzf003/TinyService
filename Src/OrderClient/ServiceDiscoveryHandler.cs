using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TinyService.Discovery.Consul;

namespace OrderClient
{
    public class ServiceDiscoveryHandler : DelegatingHandler
    {
        readonly IClusterClinet clusterClinet;
        public ServiceDiscoveryHandler(IClusterClinet clusterClinet)
            
        {
            this.clusterClinet = clusterClinet;
        }


        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            Task<HttpResponseMessage> response = null;
            try
            {

                Stopwatch sw = Stopwatch.StartNew();
                var servicename = request.RequestUri.Host;
                Console.WriteLine("请求服务信息:{0}", servicename);

                var serviceinfo = await clusterClinet.FindServiceInstanceAsync(servicename);

                request.RequestUri = new Uri($"http://{serviceinfo.ToString()}{request.RequestUri.PathAndQuery}");
                Console.WriteLine("请求Url：{0}", request.RequestUri.ToString());
                response = base.SendAsync(request, cancellationToken);
                sw.Stop();
                Console.WriteLine("请求完毕：{0}--耗时:{1} ms", response.Result.IsSuccessStatusCode, sw.ElapsedMilliseconds);

            }
            catch (Exception ex)
            {
                response = Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest));
            }

            return await response;

        }
    }
}
