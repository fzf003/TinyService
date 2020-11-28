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
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

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

        readonly IDistributedCache _cache;

        public QueryOrderHandler(IOrderRepository orderRepository, IMapper mapper, ILoggerFactory loggerFactory, IHttpContextAccessor httpContext, IDistributedCache cache)
        {
            _orderRepository = orderRepository;

            _mapper = mapper;

            _logger = loggerFactory.CreateLogger<QueryOrderHandler>();

            this.httpContext = httpContext;

            _cache = cache;
        }


        public async Task<OrderResponse> HandleAsync(GetOrderByUserNameQuery query)
        {
            var order = await this._orderRepository.GetOrdersByCustomer(query.CustomerId);
    
            return new OrderResponse(_mapper.Map<IEnumerable<OrderDto>>(order).ToList()); 
        }

        public async Task<List<OrderDto>> HandleAsync(GetOrderAll query)
        {
            Console.WriteLine(httpContext.HttpContext.Request.Path);
            var response= await _cache.GetStringAsync("all");
            if (string.IsNullOrWhiteSpace(response))
            {
                var order = await this._orderRepository.QureryOrdersAll().ConfigureAwait(false);
                var orderdto=_mapper.Map<List<OrderDto>>(order);
                await _cache.SetStringAsync("all", JsonConvert.SerializeObject(orderdto));
                return orderdto;
            }
            return await Task.Run(() => JsonConvert.DeserializeObject<List<OrderDto>>(response));
        }

        public async Task<OrderDto> HandleAsync(QueryOrderById query)
        {
            var cacheKey = $"order-{query.Id}";
            var response = await _cache.GetAsync(cacheKey);
            if (response==null)
            {
                var orderquery = await this._orderRepository.QueryOrderById(query.Id).ConfigureAwait(false);
                
                var mapperorder=_mapper.Map<OrderDto>(orderquery);

                await _cache.SetAsync($"order-{query.Id}",Encoding.UTF8.GetBytes( JsonConvert.SerializeObject(mapperorder)));
                
                return mapperorder;
            }

            return await Task.Run(() => JsonConvert.DeserializeObject<OrderDto>(Encoding.UTF8.GetString(response)));
        }


    }




    
}

