using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyService.Core;

namespace TinyService.Router
{
    public class BroadcastRouter : IRouter
    {
        private HashSet<PID> _routees;

        public HashSet<PID> GetInstances()
        {
            return _routees;
        }

        public void SetInstance(HashSet<PID> routees)
        {
            _routees = routees;
        }

        public void RouteMessage(object message)
        {

            foreach (var pid in _routees)
            {
                pid.Tell(message);
            }
        }

        public void Dispose()
        {
            if (_routees.Any())
            {
                foreach (var routeritem in _routees)
                {
                    routeritem.Stop();
                }
            }
        }
    }
}
