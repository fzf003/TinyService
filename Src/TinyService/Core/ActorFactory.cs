using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TinyService.Core
{


    public interface IActorFactory
    {
        
        PID GetActor<T>(string id = null, string address = null)
            where T : Process;

  
        PID GetActor(string id, string address = null);

     
        PID RegisterActor<T>(Func<T> actor, string id = null, string address = null)
            where T : Process;

    }

    public class ActorFactory : IActorFactory
    {

        private readonly ActorPropsRegistry actorPropsRegistry;

        readonly IServiceProvider serviceProvider;

        public ActorFactory(ActorPropsRegistry actorPropsRegistry, IServiceProvider serviceProvider)
        {
            this.actorPropsRegistry = actorPropsRegistry;

            this.serviceProvider = serviceProvider;
        
        }
     
        public PID RegisterActor<T>(Func<T> actor, string id = null, string address = null)
            where T : Process
        {

            id = id ?? typeof(T).FullName;

            return GetActor(id, address, () =>
            {
                var props = new Props().WithProducer(() => actor());
                this.actorPropsRegistry.RegisterProps<T>((prs) => props);
                return props.Spawn(id, new PID(address, id));
            });
        }

        public PID GetActor(string id, string address = null)
        {
            return GetActor(id, address, () =>
            {
                var pid = ProcessRegistry.Instance.TryGet(id, address);
                return pid.Item1;
             });
        }

        public PID GetActor<T>(string id = null, string address = null)
            where T : Process
        {

            id = id ?? typeof(T).FullName;

            return GetActor(id, address, () =>{

                return CreateActor<T>(id, () => new Props().WithProducer(() => ActivatorUtilities.CreateInstance<T>(this.serviceProvider)));
            });
        }

        public PID GetActor(string id, string address, Func<PID> create)
        {
            address = address ?? "nonhost";

            var pidId = id;
     
            var pid = new PID(address, pidId);
            var reff = ProcessRegistry.Instance.Get(pid);
            if (reff is DeadLetterProcess)
            {
                pid = create();
            }
             return pid;
        }

        internal PID CreateActor<T>(string id, Func<Props> producer)
            where T : IActor
        {
            Func<Props, Props> props;
            if (!actorPropsRegistry.RegisteredProps.TryGetValue(typeof(T), out props))
            {
                props = x => x;
            }

            var props2 = props(producer());

            if (string.IsNullOrWhiteSpace(id))
            {
                return ActorUtil.Spawn(props2);
            }
            else
            {
                 return ActorUtil.SpawnNamed(props2, id);
            }
        }
    }
}
