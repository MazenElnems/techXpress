using BusinessLogicLayer.Abstraction;
using BusinessLogicLayer.DTOs.Products;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Extensions.Session;
using PresentationLayer.VMs.ShoppingCart;

namespace PresentationLayer.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IProductManager _productManager;

        public ShoppingCartController(IProductManager productManager)
        {
            _productManager = productManager;
        }

        public IActionResult Add(int id)
        {
            ShoppingCartVM? cart = HttpContext.Session.Get<ShoppingCartVM>("Cart");

            if(cart == null)
            {
                cart = new ShoppingCartVM();
            }

            ShoppingCartItemVM? cartItem = cart.CartItems.FirstOrDefault(x => x.ProductId == id);

            if(cartItem == null)
            {
                ProductDTO? product = _productManager.GetById(id);
                
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

        public IActionResult Plus(int id)
        {
            ShoppingCartVM? cart = HttpContext.Session.Get<ShoppingCartVM>("Cart");
            ShoppingCartItemVM? cartItem = cart?.CartItems.FirstOrDefault(x => x.ProductId == id);
            if (cartItem != null && cart!= null)
            {
                cartItem.Quantity++;
                cartItem.SubTotal = cartItem.Quantity * cartItem.Price;
                cart.Total = cart.CartItems.Sum(x => x.SubTotal);

                HttpContext.Session.Set("Cart", cart);
            }
            return RedirectToAction(nameof(ViewCart));
        }

        public IActionResult Minus(int id)
        {
            ShoppingCartVM? cart = HttpContext.Session.Get<ShoppingCartVM>("Cart");
            ShoppingCartItemVM? cartItem = cart?.CartItems.FirstOrDefault(x => x.ProductId == id);
            if (cartItem != null && cart != null)
            {
                cartItem.Quantity--;
                if (cartItem.Quantity > 0)
                {
                    cartItem.SubTotal = cartItem.Quantity * cartItem.Price;
                }
                cart.Total = cart.CartItems.Sum(x => x.SubTotal);
                HttpContext.Session.Set("Cart", cart);
            }
            return RedirectToAction(nameof(ViewCart));
        }

        public IActionResult Remove(int id)
        {
            var cart = HttpContext.Session.Get<ShoppingCartVM>("Cart");
            cart?.CartItems?.RemoveAll(x => x.ProductId == id);
            HttpContext.Session.Set("Cart", cart);

            return RedirectToAction(nameof(ViewCart));
        }
    }
}
