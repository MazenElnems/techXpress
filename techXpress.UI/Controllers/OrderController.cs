using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using techXpress.Services.Abstraction;
using techXpress.Services.DTOs.Orders;
using techXpress.UI.Extensions.Session;
using techXpress.UI.VMs.ShoppingCart;

namespace techXpress.UI.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderManger _orderManger;
        public OrderController(IOrderManger orderManger)
        {
            _orderManger = orderManger;
        }

        [Authorize]
        public IActionResult Create()
        {


            //  1) Get the cart from session
            //  2) create createorderDTO object and send it to the service
            //  3) within the service 
                    // 1) 

            return View();

            //var cart = HttpContext.Session.Get<ShoppingCartVM>("Cart");

            //if (cart != null)
            //{
            //    var productQuantities = cart.CartItems
            //        .ToDictionary(p => p.ProductId, p => p.Quantity);
                
            //    var productPrices = cart.CartItems
            //        .ToDictionary(p => p.ProductId, p => p.Price);

            //    CreateOrderDTO orderDto = new CreateOrderDTO
            //    {
            //        ProductQuantities = productQuantities,
            //        ProductPrices = productPrices,
            //        TotalAmount = cart.Total
            //    };

            //    _orderManger.PlaceOrder(orderDto);

            //}

            //return RedirectToAction(nameof(Demo));

        }

        public IActionResult Demo()
        {
            return View();
        }
    }
}
