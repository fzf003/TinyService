using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace TinyService.Core
{
    public delegate PID Spawner(string id, Props props, PID parent);

    public delegate void Sender(PID target);

    #region 系统消息
    public class Started
    {
        public static Started Instance = new Started();
        private Started() { }
    }

    #endregion

    public class Props
    {
        Spawner _spawner;

        public Func<Process> Producer { get; private set; }


        public IList<Func<Sender, Sender>> SenderMiddleware  =new  List<Func<Sender, Sender>>();

        public Sender SenderMiddlewareChain { get; private set; }

        public Spawner Spawner
        {
            get
            {
                return _spawner ?? DefaultSpawner;
            }
            private set { _spawner = value; }
        }
 

        static PID DefaultSpawner(string name, Props props, PID parent)
        {
             Process process = new LocalProcess();
             var ps= ProcessRegistry.Instance.TryAdd(name,  process);
             return ps.Item1;
        }

        public Props WithSenderMiddleware(params Func<Sender, Sender>[] middleware)
        {
          return  Copy(props =>
                {
                    props.SenderMiddleware = SenderMiddleware.Concat(middleware).ToList();
                    
                    props.SenderMiddlewareChain = props.SenderMiddleware.Reverse()
                         
                                                       .Aggregate((Sender) null, (inner, outer) => outer(inner));
                });
        }

        public Props WithSpawner(Spawner spawner)
        {
            return Copy(props => props.Spawner = spawner);
        }

        internal PID Spawn(string name, PID parent)
        {


            return new Spawner((id, props, pid) =>
            {
                if (props.Producer == null)
                    return new PID();
 
                var info = ProcessRegistry.Instance.TryAdd(id, props.Producer());
                if(info.Item2)
                {
                    info.Item1.Tell(Started.Instance);
                    return info.Item1;
                }
                else
                {
                    info.Item1.Tell(Started.Instance);
                    return info.Item1;
                }
 
            })(name, this, null);
        }

        public Props WithProducer(Func<Process> producer)
        {
            return Copy(props => props.Producer = producer);
        }

        private Props Copy(Action<Props> mutator)
        {
            var props = new Props
            {
                Producer = Producer,
                Spawner = Spawner,
                SenderMiddlewareChain=SenderMiddlewareChain
            };
            mutator(props);
            return props;
        }

    }
}
