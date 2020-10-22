using AutoMapper;
using Microsoft.Extensions.Logging;
using Ordering.Application.Mapper;
using Ordering.Application.Query;
using Ordering.Domain;
using Ordering.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TinyService.Cqrs;
using System.Linq;
using Ordering.Application.Dto;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Http;

namespace Ordering.Application.Query
{
    public class QueryOrderHandler : IQueryHandler<GetOrderByUserNameQuery, OrderResponse>,
                                     IQueryHandler<QueryOrderById, OrderDto>,
                                     IQueryHandler<GetOrderAll,List<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;

        private readonly IMapper _mapper;

        readonly ILogger<QueryOrderHandler> _logger;

        readonly IHttpContextAccessor httpContext;

        public QueryOrderHandler(IOrderRepository orderRepository, IMapper mapper, ILoggerFactory loggerFactory, IHttpContextAccessor httpContext)
        {
            _orderRepository = orderRepository;

            _mapper = mapper;

            _logger = loggerFactory.CreateLogger<QueryOrderHandler>();

            this.httpContext = httpContext;
        }


        public async Task<OrderResponse> HandleAsync(GetOrderByUserNameQuery query)
        {
            var order = await this._orderRepository.GetOrdersByCustomer(query.CustomerId);
    
            return new OrderResponse(_mapper.Map<IEnumerable<OrderDto>>(order).ToList()); 
        }

        public async Task<List<OrderDto>> HandleAsync(GetOrderAll query)
        {
            Console.WriteLine(httpContext.HttpContext.Request.Path);

            var order = await this._orderRepository.QureryOrdersAll().ConfigureAwait(false);

            return _mapper.Map<List<OrderDto>>(order);
        }

        public async Task<OrderDto> HandleAsync(QueryOrderById query)
        {
            var orderquery= await this._orderRepository.QueryOrderById(query.Id).ConfigureAwait(false);

            return _mapper.Map<OrderDto>(orderquery);
        }

       
    }




    
}

