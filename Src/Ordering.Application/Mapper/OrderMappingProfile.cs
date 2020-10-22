using AutoMapper;
using Ordering.Application.Commands;
using Ordering.Application.Dto;
using Ordering.Application.Query;
using Ordering.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Ordering.Application.Mapper
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
 
            CreateMap<ProductLine, OrderLine>().ReverseMap();

            CreateMap<CreateOrderCommand, Order>()
                .ForMember(odt=>odt.Items,opt=>opt.MapFrom(src=>src.ProductLines))
                .ForMember(odt=>odt.TotalAmount,opt=>opt.MapFrom(src=>src.ProductLines.Sum(p=>p.Quantity*p.UnitPrice)))
                .ReverseMap();

            CreateMap<OrderLine, OrderLineDto>().ReverseMap();
            CreateMap<Order, OrderDto>()
                .ForMember(odt=>odt.Id,opt=>opt.MapFrom(src=>src.Id))
                .ForMember(odt=>odt.OrderLines,opt=>opt.MapFrom(src=>src.Items))
                .ReverseMap();
        }
    }
    
}
