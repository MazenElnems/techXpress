using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using techXpress.Services.Abstraction;
using techXpress.Services.DTOs.Orders;
using techXpress.UI.ActionRequests;
using techXpress.UI.Extensions.Session;
using techXpress.UI.VMs.ShoppingCart;

namespace techXpress.UI.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderManger _orderManger;
        public OrderController(IOrderManger orderManger)
        {
            _orderManger = orderManger;
        }

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
                // 4. stripe payment logic
                // 5. redirect the user to 

                ShoppingCartVM? cart = HttpContext.Session.Get<ShoppingCartVM>("Cart");
                List<ShoppingCartItemVM> products = cart!.CartItems;

                CreateOrderDTO orderDto = request.ToDto();
                orderDto.TotalAmount = cart.Total;
                orderDto.ProductQuantities = products
                    .ToDictionary(p => p.ProductId, p => p.Quantity);

                int orderId = _orderManger.PlaceOrder(orderDto);

                //Stripe payment logic

                var domain = "https://localhost:7294";
                var options = new SessionCreateOptions
                {

                    LineItems = products.Select(p => new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = p.ProductName,
                                //Images = new List<string> { p.ImageUrl },
                            },
                            UnitAmountDecimal = (decimal)p.Price * 100,
                        },
                        Quantity = p.Quantity,
                    }).ToList(),

                    Mode = "payment",
                    SuccessUrl = $"{domain}/Order/Success?id={orderId}",
                    CancelUrl = $"{domain}/ShoppingCart/ViewCart",
                };

                var service = new SessionService();
                Session session = service.Create(options);

                // 6. update order with sessionId
                _orderManger.UpdateOrder(new UpdateOrderDTO
                {
                    Id = orderId,
                    SessionId = session.Id
                });

                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }
            ModelState.AddModelError("Order Data Error", "Can't Place Order");
            return View(request);
        }

        [HttpGet]
        public IActionResult Success(int id)
        {
            OrderDto orderDto = _orderManger.GetOrderById(id)!;
            
            SessionService service = new SessionService();
            Session session = service.Get(orderDto.SessionId);
            
            _orderManger.UpdateOrder(new UpdateOrderDTO
            {
                Id = id,
                PaymentId = session.PaymentIntentId
            });


            // clear the cart
            HttpContext.Session.Remove("Cart");

            return View(id);
        }
    }
}
