using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyService.Core;
using System.Threading;
using TinyService.Utils;
using System.Threading.Tasks;
using TinyService.RequestResponse;
namespace TinyService.Router
{
    public interface IRouter : IDisposable
    {
        HashSet<PID> GetInstances();

        void SetInstance(HashSet<PID> routees);

        void RouteMessage(object message);

    }
 

    public class PoolRouter : IDisposable
    {
        private readonly int _poolSize;

        private readonly IRouter _router;

        public PoolRouter(Func<string, PID> func, int poolSize = 5, RouterEnum routerconfig = RouterEnum.RoundRobin)
        {
            _poolSize = poolSize;

            if (routerconfig == RouterEnum.RoundRobin)
            {
                _router = new RoundRobinRouter();
            }
            else
            {
                _router = new BroadcastRouter();
            }

            var routees = Enumerable.Range(0, _poolSize).Select(x => func(string.Format("Pool-{0}", ProcessRegistry.Instance.NextId())));
           
            _router.SetInstance(new HashSet<PID>(routees));

        }


        public void Forward(object message)
        {
            _router.RouteMessage(message);
        }


        public void Dispose()
        {
            Console.WriteLine("路由释放...");
            _router.Dispose();
        }
    }

    public enum RouterEnum
    {

        RoundRobin,
        Broadcast,
        Hash

    }
  
}
