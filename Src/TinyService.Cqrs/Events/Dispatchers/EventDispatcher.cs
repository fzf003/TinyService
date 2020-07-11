using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TinyService.Cqrs.Events.Dispatchers
{
  

    public interface IEventDispatcher
    {
        Task PublishAsync<T>(T @event) where T : class, IEvent;
    }

    internal sealed class EventDispatcher : IEventDispatcher
    {
        private readonly IServiceScopeFactory _serviceFactory;

        public EventDispatcher(IServiceScopeFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        public async Task PublishAsync<T>(T @event) where T : class, IEvent
        {
            using (var scope = _serviceFactory.CreateScope())
            {
                var handlers = scope.ServiceProvider.GetServices<IEventHandler<T>>();
                foreach (var handler in handlers)
                {
                    await handler.HandleAsync(@event);
                }
            }
        }
    }
}
