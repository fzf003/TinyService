using Microsoft.EntityFrameworkCore;
using Ordering.Domain;
using Ordering.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(OrderContext dbContext) : base(dbContext)
        {
        }

        public async Task CacnelOrder(int OrderNumer)
        {
           var order= await this._dbContext.Orders.Include(p=>p.Items).FirstOrDefaultAsync(p => p.Id == OrderNumer);
            order?.Cancel();
            order?.Items?.ForEach(p => p?.UpdateTime());
        }

        public Task CommitAsync()
        {
            return this._dbContext.SaveChangesAsync();
        }

        public  Task CreateOrder(Order order)
        {
           this._dbContext.Orders.Add(order);

            return Task.CompletedTask;
        }

        public async Task<IEnumerable<Order>> GetOrdersByCustomer(string customerId)
        {
            var orderList = await _dbContext.Orders
                                          .Include(p=>p.Items)
                                          .Where(p=>p.CustomerId==customerId)
                                          .ToListAsync();

            return orderList;
        }

        public async Task<Order> QueryOrderById(long Id)
        {
            return await _dbContext.Orders
                                    .Include(p => p.Items)
                                    .FirstOrDefaultAsync(p => p.Id == Id);
        }

        public async Task<IEnumerable<Order>> QureryOrdersAll()
        {
            var orderList = await _dbContext.Orders
                                         .Include(p => p.Items)
                                         .ToListAsync().ConfigureAwait(false);
             return orderList;
        }

          
    }
}
