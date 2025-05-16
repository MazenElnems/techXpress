using Microsoft.AspNetCore.Mvc;
using techXpress.Services.Abstraction;
using techXpress.Services.DTOs.Products;
using techXpress.UI.Extensions.Session;
using techXpress.UI.VMs.WishList;

namespace techXpress.UI.Controllers
{
    public class WishlistController : Controller
    {
        private readonly IProductManager _productManager;

        public WishlistController(IProductManager productManager)
        {
            _productManager = productManager;
        }

        public IActionResult ViewWishlist()
        {
            WishListVM? wishList = HttpContext.Session.Get<WishListVM>("wishlist") ?? new WishListVM();
            return View(wishList);
        }

        public IActionResult Add(int id)
        {
            WishListVM? wishList = HttpContext.Session.Get<WishListVM>("wishlist");
            if (wishList == null)
            {
                wishList = new WishListVM();
            }
            // Check if item already exists in wishlist
            WishListItemVM? existingItem = wishList.WishListItems.FirstOrDefault(x => x.ProductId == id);
            if (existingItem == null)
            {
                ProductDTO? product = _productManager.GetById(id);
                if (product != null)
                {
                    WishListItemVM wishListItem = new WishListItemVM
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Price = product.Price,
                        ImageUrl = product.Image,
                        AverageRating = product.AverageRating
                    };
                    wishList.WishListItems.Add(wishListItem);
                    HttpContext.Session.Set("wishlist", wishList);
                    return Json(new { success = "Item added to your WishList" });
                }
            }
            else
            {
                return Json(new { Info = "This item is already in your WishList" });
            }

            return Json(new { error = "Can't add this Item to WishList" });
        }

        public IActionResult Remove(int id)
        {
            WishListVM? wishList = HttpContext.Session.Get<WishListVM>("wishlist");

            if (wishList != null)
            {
                wishList.WishListItems.RemoveAll(x => x.ProductId == id);
                HttpContext.Session.Set("wishlist", wishList);
                TempData["Success"] = "Item removed from your WishList";
            }

            return RedirectToAction(nameof(ViewWishlist));
        }

        [HttpPost]
        public IActionResult Clear()
        {
            HttpContext.Session.Remove("wishlist");
            TempData["Success"] = "Your WishList has been cleared";
            return RedirectToAction(nameof(ViewWishlist));
        }

        [HttpPost]
        public IActionResult AddToCart(int id)
        {
            WishListVM? wishList = HttpContext.Session.Get<WishListVM>("wishlist");
            
            if (wishList != null)
            {
                // Remove from wishlist after adding to cart
                wishList.WishListItems.RemoveAll(x => x.ProductId == id);
                HttpContext.Session.Set("wishlist", wishList);
            }

            TempData["Success"] = "Item moved to your shopping cart";
            
            // Redirect to ShoppingCart controller's Add action
            return RedirectToAction("Add", "ShoppingCart", new { id });
        }
    }
}
