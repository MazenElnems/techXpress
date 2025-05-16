using techXpress.Services.Abstraction;
using techXpress.Services.DTOs.Products;
using Microsoft.AspNetCore.Mvc;
using techXpress.UI.Extensions.Session;
using techXpress.UI.VMs.ShoppingCart;
using Microsoft.AspNetCore.Authorization;

namespace techXpress.UI.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IProductManager _productManager;

        public ShoppingCartController(IProductManager productManager)
        {
            _productManager = productManager;
        }

        public async Task<IActionResult> Add(int id)
        {
            ShoppingCartVM? cart = HttpContext.Session.Get<ShoppingCartVM>("Cart");

            if (cart == null)
            {
                cart = new ShoppingCartVM();
            }

            ShoppingCartItemVM? cartItem = cart.CartItems.FirstOrDefault(x => x.ProductId == id);

            if (cartItem == null)
            {
                ProductDTO? product = await _productManager.GetByIdAsync(id);

                cartItem = new ShoppingCartItemVM
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    ImageUrl = product.Image,
                    Quantity = 1,
                    SubTotal = product.Price,
                };

                cart.CartItems.Add(cartItem);

                cart.Total = cart.CartItems.Sum(x => x.SubTotal);

                HttpContext.Session.Set("Cart", cart);
            }

            return RedirectToAction(nameof(ViewCart));
        }

        public IActionResult ViewCart()
        {
            ShoppingCartVM? cart = HttpContext.Session.Get<ShoppingCartVM>("Cart");

            return View(cart);
        }

        public IActionResult Remove(int id)
        {
            var cart = HttpContext.Session.Get<ShoppingCartVM>("Cart");
            cart?.CartItems?.RemoveAll(x => x.ProductId == id);
            HttpContext.Session.Set("Cart", cart);

            return RedirectToAction(nameof(ViewCart));
        }

        [HttpPatch("cart-item/minus/{id}")]
        public IActionResult Minus(int id)
        {
            ShoppingCartVM? cart = HttpContext.Session.Get<ShoppingCartVM>("Cart");
            ShoppingCartItemVM? cartItem = cart?.CartItems.FirstOrDefault(x => x.ProductId == id);

            if (cartItem != null && cart != null)
            {
                if (cartItem.Quantity <= 1)
                {
                    return Json(new { success = false, message = "Quantity cannot be less than 1" });
                }

                cartItem.Quantity--;
                cartItem.SubTotal = cartItem.Quantity * cartItem.Price;
                cart.Total = cart.CartItems.Sum(x => x.SubTotal);
                HttpContext.Session.Set("Cart", cart);
                return Json(new { success = true, cartItem.SubTotal, cartItem.Quantity, cart.Total });
            }
            return Json(new { success = false, message = "Item not found in cart." });
        }

        [HttpPatch("cart-item/plus/{id}")]
        public IActionResult Plus(int id)
        {
            ShoppingCartVM? cart = HttpContext.Session.Get<ShoppingCartVM>("Cart");
            ShoppingCartItemVM? cartItem = cart?.CartItems.FirstOrDefault(x => x.ProductId == id);

            if (cartItem != null && cart != null)
            {
                if (cartItem.Quantity < 1)
                {
                    return Json(new { success = false, message = "Invalid quantity" });
                }

                cartItem.Quantity++;
                cartItem.SubTotal = cartItem.Quantity * cartItem.Price;
                cart.Total = cart.CartItems.Sum(x => x.SubTotal);

                HttpContext.Session.Set("Cart", cart);
                return Json(new { success = true, cartItem.SubTotal, cartItem.Quantity, cart.Total });
            }
            return Json(new { success = false, message = "Item not found in cart." });
        }


    }
}
