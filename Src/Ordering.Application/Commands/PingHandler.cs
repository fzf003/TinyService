using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Event;
using Ordering.Domain;
using Ordering.Domain.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Application.Commands
{
    public class PingHandler : IRequestHandler<CreateOrderCommand, string>
    {
         

        readonly IMediator _mediator;
        readonly IOrderRepository _orderRepository;
        readonly IMapper _mapper;

        public PingHandler(IMediator mediator, IOrderRepository orderRepository, IMapper mapper)
        {

            _mediator = mediator;
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<string> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            Console.WriteLine("Miss");
          

            var order = _mapper.Map<Order>(request);

            await this._orderRepository.CreateOrder(order);


           await this._mediator.Publish(new OrderCreated
            {
                Order = order
            }); ;

      

           await this._orderRepository.CommitAsync();


            return Guid.NewGuid().ToString("N");
        }
    }

    public class PubEventHandle : INotificationHandler<OrderCreated>
    {

        readonly ILogger<PubEventHandle> _logger;

        public PubEventHandle(ILogger<PubEventHandle> logger)
        {
            _logger = logger;
        }

        public Task Handle(OrderCreated @event, CancellationToken cancellationToken)
        {
            //throw new Exception("ddd");
            _logger.LogInformation($"用户:{@event.Order.CustomerId}创建订单:{@event.Order.TotalAmount}成功！");

            return Task.CompletedTask;
        }
    }
}
