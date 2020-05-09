using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TinyService.Core
{
    public class ActorPropsRegistry
    {
        internal readonly Dictionary<Type, Func<Props, Props>> RegisteredProps = new Dictionary<Type, Func<Props, Props>>();

        public ActorPropsRegistry()
        {
        }

        public void RegisterProps<T>(Func<Props, Props> props) where T : IActor
        {
            RegisteredProps.Add(typeof(T), props);
        }

        public void RegisterProps(Type actorType,Func<Props, Props> props) 
        {
            if (!typeof(IActor).GetType().IsAssignableFrom(actorType))
            {
                throw new InvalidOperationException(string.Format("Type {0} must implement {1}", actorType.FullName, typeof(IActor).FullName));
            }
            RegisteredProps.Add(actorType, props);
        }
    }
}
