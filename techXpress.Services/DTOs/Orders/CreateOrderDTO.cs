using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace techXpress.Services.DTOs.Orders
{
    public class CreateOrderDTO
    {
        public decimal TotalAmount { get; set; }
        public Dictionary<int,decimal> ProductPrices{ get; set; }
        public Dictionary<int,int> ProductQuantities { get; set; }
    }
}
