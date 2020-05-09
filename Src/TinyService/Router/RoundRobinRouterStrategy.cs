using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyService.Core;
using System.Threading;

namespace TinyService.Router
{
    public class RoundRobinRouter : IRouter
    {
        private HashSet<PID> _routees;

        private List<PID> _instances;

        private int serviceindex;

        public HashSet<PID> GetInstances()
        {
            return _routees;
        }



        public void SetInstance(HashSet<PID> routees)
        {
            this._routees = routees;

            this._instances = routees.ToList();
        }

        public void RouteMessage(object message)
        {
            var res = this._instances;

            var i = serviceindex % res.Count();

            var pid = res[i];

            Interlocked.Add(ref serviceindex, 1);

            pid.Tell(message);
        }



        public void Dispose()
        {
            if (_routees.Any())
            {
                foreach (var routeritem in _instances)
                {
                    routeritem.Stop();
                }
            }
        }
    }
}
