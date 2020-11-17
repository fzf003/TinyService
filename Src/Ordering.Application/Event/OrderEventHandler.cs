using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TinyService.Cqrs.Events;

namespace Ordering.Application.Event
{
    public class OrderCreated:IEvent,INotification
    {
        public Order Order { get; set; }
    }

    public class OrderEventHandler : IEventHandler<OrderCreated>
    {

        readonly ILogger<OrderEventHandler> _logger;

        public OrderEventHandler(ILogger<OrderEventHandler> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(OrderCreated @event)
        {
           
            _logger.LogInformation($"用户:{@event.Order.CustomerId}创建订单:{@event.Order.TotalAmount}成功！");
            return Task.CompletedTask;
        }
    }
}
