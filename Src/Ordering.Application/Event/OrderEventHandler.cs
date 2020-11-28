using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Ordering.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TinyService.Cqrs.Events;

namespace Ordering.Application.Event
{
    public class OrderCreated:IEvent
    {
        public Order Order { get; set; }
    }

    public class OrderCancel:IEvent
    {
        public long OrderId { get; set; }
    }

    public class OrderEventHandler : IEventHandler<OrderCreated>,
                                     IEventHandler<OrderCancel>
    {

        readonly ILogger<OrderEventHandler> _logger;

        readonly IDistributedCache _cache;

        public OrderEventHandler(ILogger<OrderEventHandler> logger, IDistributedCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        public Task HandleAsync(OrderCreated @event)
        {
            _cache.Remove($"order-{@event.Order.Id}");
            _logger.LogInformation($"{ @event.Order.Id}-用户:{@event.Order.CustomerId}创建订单:{@event.Order.TotalAmount}成功！");
            return Task.CompletedTask;
        }

        public Task HandleAsync(OrderCancel @event)
        {
            _cache.Remove($"order-{@event.OrderId}");

            _logger.LogInformation($"{@event.OrderId}取消订单成功！");

            return Task.CompletedTask;
        }
    }
}
