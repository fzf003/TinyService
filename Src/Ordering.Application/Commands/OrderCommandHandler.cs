using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Ordering.Application.Commands;
using Ordering.Application.Event;
using Ordering.Domain;
using Ordering.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TinyService.Cqrs.Commands;
using TinyService.Cqrs.Events.Dispatchers;

namespace Ordering.Application.Commands
{
    public class OrderCommandHandler : ICommandHandler<CreateOrderCommand>,ICommandHandler<CancelOrderCommand>
    {

        readonly IOrderRepository _orderRepository;

        readonly ILogger<OrderCommandHandler> _logger;

        readonly IMapper _mapper;

        readonly IHttpContextAccessor httpContext;

        readonly IEventDispatcher _eventDispatcher;

        public OrderCommandHandler(IOrderRepository orderRepository, ILogger<OrderCommandHandler> logger, IMapper mapper, IHttpContextAccessor httpContext, IEventDispatcher eventDispatcher)
        {
            _orderRepository = orderRepository;

            _logger = logger;

            _mapper = mapper;

            this.httpContext = httpContext;

            _eventDispatcher = eventDispatcher;
        }

        public async Task HandleAsync(CreateOrderCommand command)
        {
            var order=_mapper.Map<Order>(command);

            await this._orderRepository.CreateOrder(order);
 
            await this._orderRepository.CommitAsync();
 
            await _eventDispatcher.PublishAsync(new OrderCreated
            {
                Order = order
            });

            _logger.LogInformation("订单创建完成.....");
        }

        public async  Task HandleAsync(CancelOrderCommand command)
        {
            await this._orderRepository.CacnelOrder(command.OrderNumber);

            await this._orderRepository.CommitAsync();

            await _eventDispatcher.PublishAsync(new OrderCancel
            {
                OrderId = command.OrderNumber
            });
        }
    }
}
