using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        [Authorize(Roles = UserRole.Admin)]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = UserRole.Admin)]
        [HttpGet("api/orders")]
        public IActionResult GetAllOrders(string? orderStatus)
        {
            IEnumerable<GetAllOrdersDto> orders = _orderManger.GetAllOrdersWithUsers()
                .Where(o => string.IsNullOrEmpty(orderStatus) || o.OrderStatus == orderStatus);
            return Json(new { data = orders });
        }

        [Authorize(Roles = $"{UserRole.Seller},{UserRole.Customer}")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = $"{UserRole.Seller},{UserRole.Customer}")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderActionRequest request)
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

                int orderId = await _orderManger.PlaceOrderAsync(orderDto);

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
                await _orderManger.UpdateOrderAsync(new UpdateOrderDTO
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
        public IActionResult Update(int id)
        {
            UpdateOrderActionRequest? order = _orderManger.GetOrderByIdWithDetails(id)
                ?.ToDto();

            ViewBag.OrderStatusOptions = new SelectList(new[]
            {
                "Pending", "Approved", "InProgress", "Canceled", "Shipped"
            });
            ViewBag.orderId = id;

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id,UpdateOrderActionRequest request)
        {
            if (ModelState.IsValid)
            {
                UpdateOrderDTO orderDto = request.ToUpdateOrderDto();
                orderDto.Id = id;

                await _orderManger.UpdateOrderAsync(orderDto);

                TempData["successNotification"] = "Order updated successfully";
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("Order Data Error", "Can't Update Order");
            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> Success(int id)
        {
            OrderDto? orderDto = _orderManger.GetOrderById(id);

            SessionService service = new SessionService();
            Session session = service.Get(orderDto.SessionId);
            
            await _orderManger.UpdateOrderAsync(new UpdateOrderDTO
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
