using DataAccessLayer.Entities.Enums;
using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entities
{
    public class Order
    {
        public int OrderId { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime? ShippingDate { get; set; }
        public string? Carrier { get; set; }

        // Foreign keys
        public int UserId { get; set; }
        public int PaymentId { get; set; }
        public int? CouponId { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Payment Payment { get; set; }
        public Coupon Coupon { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}