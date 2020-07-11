using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TinyService.Cqrs.Events
{
    public interface IEvent
    {

    }

    public interface IEventHandler<in TEvent> where TEvent : class, IEvent
    {
        Task HandleAsync(TEvent @event);
    }
}
