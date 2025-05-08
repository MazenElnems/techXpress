using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using techXpress.Services.DTOs.Orders;

namespace techXpress.Services.Abstraction
{
    public interface IOrderManger
    {
        Task<int> PlaceOrderAsync(CreateOrderDTO orderDto);
        Task UpdateOrderAsync(UpdateOrderDTO orderDto);
        OrderDto? GetOrderById(int id);
        OrderWithDetailsDto? GetOrderByIdWithDetails(int id);
        IEnumerable<OrderVM> GetAllOrdersWithUsers();
        IEnumerable<OrderVM> GetAllOrdersByUserId(Guid userId);
    }
}
