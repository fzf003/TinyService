using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TinyService.Core
{
    public delegate Process Receive();

    public static class ActorUtil
    {

        public static Props FromProducer(Func<Process> producer)
        {
            return new Props().WithProducer(producer);
        }

        public static Props FromFunc(Receive receive)
        {
            return FromProducer(() => receive());
        }

        public static PID Spawn(Props props)
        {
            var name = ProcessRegistry.Instance.NextId();
            return SpawnNamed(props, name);
        }

        public static PID SpawnPrefix(Props props, string prefix)
        {
            var name = string.Format("{0}/{1}", prefix, ProcessRegistry.Instance.NextId());
            return SpawnNamed(props, name);
        }

        public static PID SpawnNamed(Props props, string name)
        {
            return props.Spawn(name, null);
        }
    }

}
