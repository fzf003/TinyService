using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TinyService.Core;
using System.Threading.Tasks;
using System.Collections;

namespace TinyService.Router
{
    



    public interface ILoadBalancer
    {
        PID GetServiceEndpoint(CancellationToken ct = default(CancellationToken));
    }

    public class RoundRobinLoadBalancer : ILoadBalancer
    {

        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        private int _index;

        readonly List<PID> servicestore;

        public RoundRobinLoadBalancer(List<PID> servicestore)
        {
            this.servicestore = servicestore;
        }

        public PID GetServiceEndpoint(CancellationToken ct = default(CancellationToken))
        {
            var endpoints = this.servicestore;

            if (endpoints.Count == 0)
            {
                return null;
            }

            _lock.Wait(ct);
            try
            {
                if (_index >= endpoints.Count)
                {
                    _index = 0;
                }
                var uri = endpoints[_index];
                _index++;

                return uri;
            }
            finally
            {
                _lock.Release();
            }
        }
    }


    



    
}
