using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.Repository
{
    public interface IOrderRepository : IRepository<Order>
    {

        Task CreateOrder(Order order);

        Task<IEnumerable<Order>> GetOrdersByCustomer(string customerId);

        Task<Order> QueryOrderById(long Id);

        Task<IEnumerable<Order>> QureryOrdersAll();

        Task CacnelOrder(int OrderNumer);

        Task CommitAsync();
     
    }


}
