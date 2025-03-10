using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entities
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public string TransactionId { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public decimal Amount { get; set; }

    }
}