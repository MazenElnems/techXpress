using BusinessLogicLayer.Abstraction;
using BusinessLogicLayer.DTOs.CategoryDTOs;
using BusinessLogicLayer.DTOs.Products;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.ActionRequests;
using PresentationLayer.VMs.Category;
using PresentationLayer.VMs.Products;
using System.Collections.Generic;

namespace PresentationLayer.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductManager _productManager;
        private readonly ICategoryManager _categoryManager;
        private readonly IFilesService _filesService;
        public ProductController(IProductManager productManager, IFilesService filesService, ICategoryManager categoryManager)
        {
            _productManager = productManager;
            _filesService = filesService;
            _categoryManager = categoryManager;
        }

        [HttpPost]
        public IActionResult Index(string searchTerm, string searchBy,
            decimal? minPrice, decimal? maxPrice,bool all ,List<int> selectedCategories)
        {
            IEnumerable<ProductVM> productVMs = _productManager
                .GetProductsByFilter(searchTerm,searchBy,minPrice,maxPrice, selectedCategories,all)
                .Select(p => p.ToProductVM());

            ViewData["SearchTerm"] = searchTerm;
            ViewData["SearchBy"] = searchBy;
            ViewData["MinPrice"] = $"{minPrice}";
            ViewData["MaxPrice"] = $"{maxPrice}";
            ViewData["selectedCategories"] = selectedCategories;
            ViewData["all"] = all;

            IEnumerable<CategoryVM> categoryVMs = _categoryManager
            .GetAll()
            .Select(c => c.ToVM());

            ViewData["Categories"] = categoryVMs;

            return View(productVMs);
        }

        [HttpGet]
        public IActionResult Index()
        {
            IEnumerable<ProductVM> productVMs = _productManager
                .GetAll()
                .Select(p => p.ToProductVM())
                .ToList();

            IEnumerable<CategoryVM> categoryVMs = _categoryManager
                .GetAll()
                .Select(c => c.ToVM());

            ViewData["Categories"] = categoryVMs;
            ViewData["all"] = true;


            return View(productVMs);
        }

        [HttpGet]
        public IActionResult Create()
        {
            IEnumerable<CategoryDTO> categoryDTOs = _categoryManager.GetAll().ToList();
            ViewData["Categories"] = categoryDTOs;
            return View(new CreateProductActionRequest());
        }

        [HttpPost]
        public IActionResult Create(CreateProductActionRequest productActionRequest)
        {
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + productActionRequest.Image.FileName;
            _filesService.Upload($"wwwroot/Images/{uniqueFileName}", productActionRequest.Image);

            ProductDTO productDTO = new ProductDTO
            {
                Name = productActionRequest.Name,
                Image = uniqueFileName,
                Description = productActionRequest.Description,
                StockQuantity = productActionRequest.StockQuantity,
                Price = productActionRequest.Price,
                CategoryId = productActionRequest.CategoryId
            };

            _productManager.Create(productDTO);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int id)
        {
            ProductDTO? productDTO = _productManager.GetById(id);

            if(productDTO != null)
            {
                return View(productDTO.ToProductVM());
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
