using System;

namespace DataAccessLayer.Entities
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        // Foreign keys
        public int ProductId { get; set; }
        public int OrderId { get; set; }

        // Navigation properties
        public Product Product { get; set; }
        public Order Order { get; set; }
    }
}