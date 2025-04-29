using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using techXpress.DataAccess.Entities;

namespace techXpress.Services.DTOs.Orders
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string? SessionId { get; set; }
        public string? PaymentId { get; set; }
        public string? Carrier { get; set; }
        public string? TrackingNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ShippingDate { get; set; }

    }

    // extenstion method 
    // from order to dto
    public static class OrderDtoExtensions
    {
        public static OrderDto ToDto(this Order order)
        {
            return new OrderDto
            {
                Id = order.OrderId,
                SessionId = order.SessionId,
                PaymentId = order.PaymentId,
                Carrier = order.Carrier,
                TrackingNumber = order.TrackingNumber,
                OrderDate = order.OrderDate,
                ShippingDate = order.ShippingDate
            };
        }
    }
}
