using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using TinyService.Cqrs.Commands;

namespace Ordering.Application.Commands
{
    public class CreateOrderCommand : ICommand, IRequest<string>
    {
        public string CustomerId { get; set; }
        public decimal TotalPrice { get; set; }

        public List<ProductLine> ProductLines { get; set; } = new List<ProductLine>();
     }

    public class ProductLine
    {
        public string ProductId { get; set; }
        public string Name { get; set; }


        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }


    }

}
