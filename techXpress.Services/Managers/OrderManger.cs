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

        public OrderDto? GetOrderById(int id)
        {
            Order? order = _unitOfWork.OrderRepository.GetById(o => o.OrderId == id);
            if(order != null)
            {
                return order.ToDto();
            }
            return null;
        }

        public int PlaceOrder(CreateOrderDTO orderDto)
        {
            Order order = orderDto.ToOrder();

            // initialize order
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
            return order.OrderId;
        }

        public void UpdateOrder(UpdateOrderDTO orderDto)
        {
            Order? order = _unitOfWork.OrderRepository.GetById(o => o.OrderId == orderDto.Id);
            if (order == null)
            {
                throw new Exception("Order not found");
            }

            // TODO: Update order properties
            order.OrderStatus = orderDto.OrderStatus ?? order.OrderStatus;
            order.SessionId = orderDto.SessionId ?? order.SessionId;
            order.Carrier = orderDto.Carrier ?? order.Carrier;
            order.TrackingNumber = orderDto.TrackingNumber ?? order.TrackingNumber;
            order.ShippingDate = orderDto.ShippingDate ?? order.ShippingDate;

            // add payment row in payment table
            if(order.PaymentId != null)
            {
                order.Payment = new Payment
                {
                    PaymentId = orderDto.PaymentId,
                    Amount = order.TotalAmount,
                    PaymentDate = DateTime.Now
                };
                order.PaymentId = orderDto.PaymentId;
            }
            _unitOfWork.Save();
        }
    }
}
