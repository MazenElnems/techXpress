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
                
                if (product == null)
                {
                    TempData["errorNotification"] = "Product not found.";
                    return RedirectToAction("Index", "Home");
                }
                
                if (product.StockQuantity <= 0)
                {
                    TempData["errorNotification"] = "Product is out of stock.";
                    return RedirectToAction("Index", "Home");
                }
                
                cartItem = new ShoppingCartItemVM
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    ImageUrl = product.Image,
                    Quantity = 1,
                    SubTotal = product.Price,
                    StockQuantity = product.StockQuantity
                };

                cart.CartItems.Add(cartItem);
            }
            else
            {
                // Product already in cart, check if we can add more
                ProductDTO? product = _productManager.GetById(id);
                
                if (product == null)
                {
                    TempData["errorNotification"] = "Product not found.";
                    return RedirectToAction(nameof(ViewCart));
                }
                
                if (cartItem.Quantity >= product.StockQuantity)
                {
                    TempData["errorNotification"] = $"Only {product.StockQuantity} items available in stock.";
                    return RedirectToAction(nameof(ViewCart));
                }
                
                cartItem.Quantity++;
                cartItem.SubTotal = cartItem.Quantity * cartItem.Price;
            }

            cart.Total = cart.CartItems.Sum(x => x.SubTotal);
            HttpContext.Session.Set("Cart", cart);
            
            TempData["successNotification"] = "Product added to cart successfully.";
            return RedirectToAction(nameof(ViewCart));
        }

        public IActionResult ViewCart()
        {
            ShoppingCartVM? cart = HttpContext.Session.Get<ShoppingCartVM>("Cart");

            // Update stock quantities for all items in cart
            if (cart != null && cart.CartItems.Any())
            {
                foreach (var item in cart.CartItems)
                {
                    ProductDTO? product = _productManager.GetById(item.ProductId);
                    if (product != null)
                    {
                        item.StockQuantity = product.StockQuantity;
                        
                        // If stock is less than quantity in cart, adjust quantity
                        if (item.Quantity > product.StockQuantity)
                        {
                            item.Quantity = product.StockQuantity;
                            item.SubTotal = item.Quantity * item.Price;
                            TempData["warningNotification"] = $"Quantity for {item.ProductName} has been adjusted due to limited stock.";
                        }
                    }
                }
                
                cart.Total = cart.CartItems.Sum(x => x.SubTotal);
                HttpContext.Session.Set("Cart", cart);
            }

            return View(cart);
        }

        public IActionResult Remove(int id)
        {
            var cart = HttpContext.Session.Get<ShoppingCartVM>("Cart");
            if (cart != null)
            {
                var itemToRemove = cart.CartItems.FirstOrDefault(x => x.ProductId == id);
                if (itemToRemove != null)
                {
                    cart.CartItems.Remove(itemToRemove);
                    cart.Total = cart.CartItems.Sum(x => x.SubTotal);
                    HttpContext.Session.Set("Cart", cart);
                    TempData["successNotification"] = "Item removed from cart successfully.";
                }
            }

            return RedirectToAction(nameof(ViewCart));
        }

        [HttpPatch("cart-item/minus/{id}")]
        public IActionResult Minus(int id)
        {
            ShoppingCartVM? cart = HttpContext.Session.Get<ShoppingCartVM>("Cart");
            ShoppingCartItemVM? cartItem = cart?.CartItems.FirstOrDefault(x => x.ProductId == id);
            
            if (cartItem != null && cart != null)
            {
                if (cartItem.Quantity > 1)
                {
                    cartItem.Quantity--;
                    cartItem.SubTotal = cartItem.Quantity * cartItem.Price;
                    cart.Total = cart.CartItems.Sum(x => x.SubTotal);
                    HttpContext.Session.Set("Cart", cart);
                    
                    return Json(new { success = true, quantity = cartItem.Quantity, subTotal = cartItem.SubTotal, total = cart.Total });
                }
                else
                {
                    return Json(new { success = false, message = "Quantity cannot be less than 1." });
                }
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
                ProductDTO? product = _productManager.GetById(id);
                
                if (product == null)
                {
                    return Json(new { success = false, message = "Product not found." });
                }
                
                if (cartItem.Quantity < product.StockQuantity)
                {
                    cartItem.Quantity++;
                    cartItem.SubTotal = cartItem.Quantity * cartItem.Price;
                    cart.Total = cart.CartItems.Sum(x => x.SubTotal);
                    HttpContext.Session.Set("Cart", cart);
                    
                    return Json(new { success = true, quantity = cartItem.Quantity, subTotal = cartItem.SubTotal, total = cart.Total });
                }
                else
                {
                    return Json(new { success = false, message = $"Only {product.StockQuantity} items available in stock." });
                }
            }
            
            return Json(new { success = false, message = "Item not found in cart." });
        }
        
        [HttpPatch("cart-item/update/{id}")]
        public IActionResult UpdateQuantity(int id, [FromQuery] int quantity)
        {
            if (quantity < 1)
            {
                return Json(new { success = false, message = "Quantity must be at least 1." });
            }
            
            ShoppingCartVM? cart = HttpContext.Session.Get<ShoppingCartVM>("Cart");
            ShoppingCartItemVM? cartItem = cart?.CartItems.FirstOrDefault(x => x.ProductId == id);
            
            if (cartItem != null && cart != null)
            {
                ProductDTO? product = _productManager.GetById(id);
                
                if (product == null)
                {
                    return Json(new { success = false, message = "Product not found." });
                }
                
                if (quantity > product.StockQuantity)
                {
                    return Json(new { success = false, message = $"Only {product.StockQuantity} items available in stock." });
                }
                
                cartItem.Quantity = quantity;
                cartItem.SubTotal = cartItem.Quantity * cartItem.Price;
                cart.Total = cart.CartItems.Sum(x => x.SubTotal);
                HttpContext.Session.Set("Cart", cart);
                
                return Json(new { success = true, quantity = cartItem.Quantity, subTotal = cartItem.SubTotal, total = cart.Total });
            }
            
            return Json(new { success = false, message = "Item not found in cart." });
        }
    }
}
