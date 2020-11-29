using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ordering.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.DataContext
{
    public class OrderContextSeed
    {
        public static async Task SeedAsync(OrderContext orderContext, ILoggerFactory loggerFactory, int? retry = 3)
        {
            int retryForAvailability = retry.Value;

            try
            {
                
                orderContext.Database.EnsureCreated();
               

                if (!orderContext.Orders.Any())
                {
                    orderContext.Orders.AddRange(GetPreconfiguredOrders());
                   
                    await orderContext.SaveChangesAsync();
                }
            }
            catch (Exception exception)
            {
                if (retryForAvailability < 5)
                {
                    retryForAvailability++;
                    var log = loggerFactory.CreateLogger<OrderContextSeed>();
                    log.LogError(exception.Message);
                    await SeedAsync(orderContext, loggerFactory, retryForAvailability);
                }
                throw;
            }
        }


       
        private static IEnumerable<Order> GetPreconfiguredOrders()
        {
            return new List<Order>()
            {
                new Order()
                {
                   Items=new List<OrderLine>()
                   {
                        new OrderLine(){
                         ProductId="001",
                         Name="Think pad 笔记本",
                         Quantity=10,
                         UnitPrice=1900
                        }
                   },
                    CustomerId="super man",
                    TotalAmount=19000,
                    Status=OrderStatus.Created
                },
                  new Order()
                {
                   Items=new List<OrderLine>()
                   {
                        new OrderLine(){
                         ProductId="002",
                         Name="相机",
                         Quantity=10,
                         UnitPrice=1300
                        }
                   },
                    CustomerId="aoto man",
                    TotalAmount=13000,
                    Status=OrderStatus.Created
                }



             };
        }
    }
}
