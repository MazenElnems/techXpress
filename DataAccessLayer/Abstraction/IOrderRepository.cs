using DataAccessLayer.Entities.Enums;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstraction
{
    public interface IOrderRepository : IRepository<Order>
    {
        IEnumerable<Order> GetOrdersByUser(int userId);
        Order? GetOrderWithDetails(int orderId);
        IEnumerable<Order> GetOrdersByStatus(OrderStatus status);
        void UpdateOrderStatus(int orderId, OrderStatus newStatus);
    }
}
