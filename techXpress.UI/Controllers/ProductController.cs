using techXpress.Services.Abstraction;
using techXpress.Services.DTOs.CategoryDTOs;
using techXpress.Services.DTOs.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using techXpress.UI.ActionRequests;
using techXpress.UI.VMs.Category;
using techXpress.UI.VMs.Products;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using techXpress.UI.Models;

namespace techXpress.UI.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductManager _productManager;
        private readonly ICategoryManager _categoryManager;
        private readonly IFilesService _filesService;
        public ProductController(IProductManager productManager,
            IFilesService filesService,
            ICategoryManager categoryManager)
        {
            _productManager = productManager;
            _filesService = filesService;
            _categoryManager = categoryManager;
        }

        [Authorize(Roles = UserRole.Admin)]
        public IActionResult Index()
        {
            IEnumerable<CategoryVM> categories = _categoryManager.GetAll()
                .Select(c => c.ToVM())
                .ToList();

            return View(categories);
        }

        [Authorize(Roles = UserRole.Admin)]
        [HttpGet("api/products")]
        public IActionResult GetAllProducts(int? categoryId)
        {
            IEnumerable<ProductVM> products = _productManager.GetAll()
                .Where(p => categoryId == null || p.CategoryId == categoryId)
                .Select(p => p.ToProductVM())
                .ToList();

            return Json(new {data= products });
        }

        [Authorize(Roles = UserRole.Admin)]
        [HttpGet]
        public IActionResult Create()
        {
            IEnumerable<SelectListItem> categoriesList = _categoryManager.GetAll()
                .Select(c => new SelectListItem(c.Name, c.CategoryId.ToString()))
                .ToList();

            CreateProductActionRequest productActionRequest = new CreateProductActionRequest()
            {
                CategoryList = categoriesList
            };

            return View(productActionRequest);
        }

        [Authorize(Roles = UserRole.Admin)]
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductActionRequest productActionRequest)
        {
            if (ModelState.IsValid)
            {
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + productActionRequest.ImageUrl.FileName;
                _filesService.Upload($"wwwroot/Images/{uniqueFileName}", productActionRequest.ImageUrl);

                ProductDTO productDTO = productActionRequest.ToDto();
                productDTO.Image = uniqueFileName;

                await _productManager.CreateProductAsync(productDTO);

                TempData["successNotification"] = "Product created successfully";

                return RedirectToAction(nameof(Index));
            }
            productActionRequest.CategoryList = _categoryManager.GetAll()
                .Select(c => new SelectListItem(c.Name, c.CategoryId.ToString()))
                .ToList();
            TempData["errorNotification"] = "Product creation failed";
            return View(productActionRequest);
        }

        [HttpGet]
        [Authorize(Roles = UserRole.Admin)]
        public async Task<IActionResult> Update(int id)
        {
            ProductDTO? product = await _productManager.GetByIdAsync(id);
            if (product != null)
            {
                UpdateProductActionRequest productActionRequest = product.ToActionRequest();
                productActionRequest.CategoryList = _categoryManager.GetAll()
                    .Select(c => new SelectListItem(c.Name, c.CategoryId.ToString()))
                    .ToList();

                return View(productActionRequest);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = UserRole.Admin)]
        public async Task<IActionResult> Update(int id, UpdateProductActionRequest request)
        {
            request.Id = id;

            if (ModelState.IsValid)
            {
                ProductDTO productDTO = request.ToDto();

                // user submits a new image file
                if (request.Image != null)
                {
                    // delete old image
                    if(System.IO.File.Exists($"wwwroot/Images/{request.ImageUrl}"))
                    {
                        System.IO.File.Delete($"wwwroot/Images/{request.ImageUrl}");
                    }

                    // save new image
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + request.Image.FileName;
                    _filesService.Upload($"wwwroot/Images/{uniqueFileName}", request.Image);
                    productDTO.Image = uniqueFileName;
                }
                else
                {
                    productDTO.Image = request.ImageUrl;
                }

                await _productManager.UpdateProductAsync(productDTO);
                TempData["successNotification"] = "Product updated successfully";
                return RedirectToAction(nameof(Index));
            }

            request.CategoryList = _categoryManager.GetAll()
                .Select(c => new SelectListItem(c.Name, c.CategoryId.ToString()))
                .ToList();
            TempData["errorNotification"] = "Product update failed";
            return View(request);
        }

        [Authorize(Roles = UserRole.Customer)]
        [HttpPost]
        public async Task<IActionResult> AddReview(int id, ProductReviewVM reviewVM)
        {
            reviewVM.CreatedAt = DateTime.Now;
            await _productManager.AddReviewAsync(id, reviewVM.ToDto());
            TempData["successNotification"] = "Review added successfully";
            return RedirectToAction("Details", "Home", new { id = id });
        }

        [HttpDelete("api/delete/{id}")]
        [Authorize(Roles = UserRole.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            ProductDTO? product = await _productManager.GetByIdAsync(id);
            
            if(product != null)
            {
                await _productManager.DeleteProductAsync(id);

                if (System.IO.File.Exists($"wwwroot/Images/{product.Image}"))
                {
                    System.IO.File.Delete($"wwwroot/Images/{product.Image}");
                }

                return Json(new { success = true, message = "Product deleted successfully" });
            }
            return Json(new { success = false, message = "Product deletion failed" });
        }

    }
}
