using Ordering.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Application.Dto
{
    public class OrderDto
    {
        public OrderDto()
        {
            this.OrderLines = new List<OrderLineDto>();
        }
        public long Id { get; set; }
        public string CustomerId { get; set; }
 
        public decimal TotalAmount { get; set; }

        public OrderStatus Status { get; set; }

        public List<OrderLineDto> OrderLines { get; set; }
    }

    public class OrderLineDto
    {
        public long OrderId { get; set; }

 
        public string ProductId { get; set; }
   
        public string Name { get; set; }
        public int Quantity { get; set; }
     
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }

    }
}
