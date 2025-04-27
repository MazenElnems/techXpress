using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using techXpress.Services.Abstraction;
using techXpress.Services.DTOs.Orders;
using techXpress.UI.ActionRequests;
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
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateOrderActionRequest request)
        {
            if (ModelState.IsValid)
            {
                // 1. get the product in the shopping cart.
                // 2. assign TotalAmount and ProductQuantities to CreateOrderDTO object
                // 3. PlaceOrder using orderManager

                ShoppingCartVM? cart = HttpContext.Session.Get<ShoppingCartVM>("Cart");
                List<ShoppingCartItemVM> products = cart!.CartItems;

                CreateOrderDTO orderDto = request.ToDto();
                orderDto.TotalAmount = cart.Total;
                orderDto.ProductQuantities = products
                    .ToDictionary(p => p.ProductId, p => p.Quantity);

                _orderManger.PlaceOrder(orderDto);

                return RedirectToAction(nameof(CheckOut));
            }
            ModelState.AddModelError("Order Data Error", "Can't Place Order");
            return View(request);
        }

        public IActionResult CheckOut()
        {
            return View();
        }
    }
}
