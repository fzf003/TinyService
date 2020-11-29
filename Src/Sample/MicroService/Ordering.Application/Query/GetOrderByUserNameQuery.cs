using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Ordering.Application.Dto;
using Ordering.Domain;
using System;
using System.Collections.Generic;
using TinyService.Cqrs;

namespace Ordering.Application.Query
{


    public class GetOrderByUserNameQuery : IQuery<OrderResponse>
    {
        public GetOrderByUserNameQuery(string customerId)
        {
            CustomerId = customerId ?? throw new ArgumentNullException(nameof(CustomerId));
        }
 
        public string CustomerId { get; set; }
    }

    public class GetOrderAll: IQuery<List<OrderDto>>
    {
        public static GetOrderAll Instance = new GetOrderAll();
    }

    public class QueryOrderById:IQuery<OrderDto>
    {
         public long Id { get; set; }
        public QueryOrderById(long id)
        {
            this.Id = id;
        }
    }


    public class OrderResponse
    {
        public OrderResponse(List<OrderDto> orderDtos)
        {
            this.Orders = orderDtos;
        }
        public List<OrderDto> Orders { get; private set; }
    }
}
