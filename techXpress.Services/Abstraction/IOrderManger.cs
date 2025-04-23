using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using techXpress.Services.DTOs.Orders;

namespace techXpress.Services.Abstraction
{
    public interface IOrderManger
    {
        void PlaceOrder(CreateOrderDTO orderDto);
    }
}
