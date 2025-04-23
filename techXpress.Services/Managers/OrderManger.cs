using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using techXpress.DataAccess.Abstraction;
using techXpress.DataAccess.Entities;
using techXpress.DataAccess.Entities.Enums;
using techXpress.Services.Abstraction;
using techXpress.Services.DTOs.Orders;

namespace techXpress.Services.Managers
{
    public class OrderManger : IOrderManger
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderManger(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void PlaceOrder(CreateOrderDTO orderDto)
        {
            Order order = new Order
            {
                OrderDate = DateTime.Now,
                OrderStatus = OrderStatus.Pending,
                TotalAmount = orderDto.TotalAmount,
                OrderDetails = orderDto.ProductQuantities
                .Select(p => new OrderDetail
                {
                    ProductId = p.Key,
                    Quantity = p.Value,
                    UnitPrice = orderDto.ProductPrices[p.Key],

                }).ToList()

            };

            _unitOfWork.OrderRepository.Create(order);
            _unitOfWork.Save();
        }
    }
}
