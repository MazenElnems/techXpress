using BusinessLogicLayer.Abstraction;
using BusinessLogicLayer.DTOs.CategoryDTOs;
using BusinessLogicLayer.DTOs.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PresentationLayer.ActionRequests;
using PresentationLayer.VMs.Category;
using PresentationLayer.VMs.Products;
using System.Collections.Generic;
using System.Text.Json;

namespace PresentationLayer.Controllers
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

        public IActionResult Index()
        {
            IEnumerable<CategoryVM> categories = _categoryManager.GetAll()
                .Select(c => c.ToVM())
                .ToList();

            ViewBag.Categories = categories;
            return View();
        }

        [HttpGet("api/products")]
        public IActionResult GetAllProducts()
        {
            IEnumerable<ProductVM> products = _productManager.GetAll()
                .Select(p => p.ToProductVM())
                .ToList();

            return Json(new {data= products });
        }


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

        [HttpPost]
        public IActionResult Create(CreateProductActionRequest productActionRequest)
        {
            if (ModelState.IsValid)
            {
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + productActionRequest.ImageUrl.FileName;
                _filesService.Upload($"wwwroot/Images/{uniqueFileName}", productActionRequest.ImageUrl);

                ProductDTO productDTO = productActionRequest.ToDto();
                productDTO.Image = uniqueFileName;

                _productManager.Create(productDTO);

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
        public IActionResult Update(int id)
        {
            ProductDTO? product = _productManager.GetById(id);
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
        public IActionResult Update(int id, UpdateProductActionRequest request)
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

                _productManager.Update(productDTO);
                TempData["successNotification"] = "Product updated successfully";
                return RedirectToAction(nameof(Index));
            }

            request.CategoryList = _categoryManager.GetAll()
                .Select(c => new SelectListItem(c.Name, c.CategoryId.ToString()))
                .ToList();
            TempData["errorNotification"] = "Product update failed";
            return View(request);
        }

        [HttpDelete("api/delete/{id}")]
        public IActionResult Delete(int id)
        {
            ProductDTO? product = _productManager.GetById(id);
            
            if(product != null)
            {
                _productManager.Delete(product);

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
