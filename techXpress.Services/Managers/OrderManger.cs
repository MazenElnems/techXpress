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

        public IEnumerable<OrderVM> GetAllOrdersWithUsers()
        {
            IEnumerable<Order> orders = _unitOfWork.OrderRepository.GetAll("User");

            return orders
                .Select(o => o.GetAllOrdersDto())
                .ToList();
        }

        public IEnumerable<OrderVM> GetAllOrdersByUserId(Guid userId)
        {
            IEnumerable<Order> orders = _unitOfWork.OrderRepository.GetOrdersByUser(userId);
            return orders
                .Select(o => o.GetAllOrdersDto())
                .ToList();
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

        public OrderWithDetailsDto? GetOrderByIdWithDetails(int id)
        {
            Order? order = _unitOfWork.OrderRepository.GetOrderWithDetails(id);
            if (order != null)
            {
                return order.ToOrderWithDetailsDto();
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

            order.OrderStatus = orderDto.OrderStatus ?? order.OrderStatus;
            order.SessionId = orderDto.SessionId ?? order.SessionId;
            order.Carrier = orderDto.Carrier ?? order.Carrier;
            order.TrackingNumber = orderDto.TrackingNumber ?? order.TrackingNumber;
            order.ShippingDate = orderDto.ShippingDate ?? order.ShippingDate;
            order.Address = orderDto.Address ?? order.Address;
            order.City = orderDto.City ?? order.City;
            order.RecipientPhoneNumber = orderDto.RecipientPhoneNumber ?? order.RecipientPhoneNumber;
            order.CouponId = orderDto.CouponId ?? order.CouponId;

            // add payment row in payment table
            if (orderDto.PaymentId != null)
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
