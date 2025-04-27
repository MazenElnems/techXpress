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
            Order order = orderDto.ToOrder();

            // intialize order
            order.OrderDate = DateTime.Now;
            order.OrderStatus = OrderStatus.Pending;
            order.TotalAmount = orderDto.TotalAmount;
           
            _unitOfWork.OrderRepository.Create(order);
            
            List<OrderDetail> orderDetails = orderDto.ProductQuantities
                .Select(p => new OrderDetail
                {
                    ProductId = p.Key,
                    Quantity = p.Value,
                    UnitPrice = _unitOfWork.ProductRepository.GetById(product => product.Id == p.Key)!.Price
                }).ToList();

            order.OrderDetails = orderDetails;

            _unitOfWork.Save();
        }
    }
}
