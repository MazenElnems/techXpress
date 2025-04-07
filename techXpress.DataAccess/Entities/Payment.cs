using System;
using System.Collections.Generic;

namespace techXpress.DataAccess.Entities
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public string TransactionId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }

    }
}