using BusinessLogicLayer.Abstraction;
using BusinessLogicLayer.DTOs.CategoryDTOs;
using BusinessLogicLayer.DTOs.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        [HttpGet]
        public IActionResult Index()
        {
            IEnumerable<ProductVM> productVMs = _productManager
                .GetAll()
                .Select(p => p.ToProductVM())
                .ToList();

            IEnumerable<CategoryVM> categoryVMs = _categoryManager
                .GetAll()
                .Select(c => c.ToVM())
                .ToList();

            ViewData["Categories"] = categoryVMs;
            ViewData["all"] = true;

            return View(productVMs);
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
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + productActionRequest.Image.FileName;
                _filesService.Upload($"wwwroot/Images/{uniqueFileName}", productActionRequest.Image);

                ProductDTO productDTO = productActionRequest.ToDto();
                productDTO.Image = uniqueFileName; 

                _productManager.Create(productDTO);

                return RedirectToAction(nameof(Index));
            }
            productActionRequest.CategoryList = _categoryManager.GetAll()
                .Select(c => new SelectListItem(c.Name, c.CategoryId.ToString()))
                .ToList();
            return View(productActionRequest);
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            ProductDTO? product = _productManager.GetById(id);
            if(product != null)
            {
                UpdateProductActionRequest productActionRequest = product.ToActionRequest();
                productActionRequest.CategoryList = _categoryManager.GetAll()
                    .Select(c => new SelectListItem(c.Name,c.CategoryId.ToString()))
                    .ToList();

                return View(productActionRequest);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Update(int id,UpdateProductActionRequest request)
        {
            request.Id = id;
            if (ModelState.IsValid)
            {
                ProductDTO productDTO = request.ToDto();

                // if user submit a new image file
                if(request.Image != null)
                {
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + request.Image.FileName;
                    _filesService.Upload($"wwwroot/Images/{uniqueFileName}", request.Image);

                    productDTO.Image = uniqueFileName;
                }
                else
                {
                    productDTO.Image = request.ImageName;
                }

                _productManager.Update(productDTO);
                return RedirectToAction(nameof(Index));
            }
            request.CategoryList = _categoryManager.GetAll()
                    .Select(c => new SelectListItem(c.Name, c.CategoryId.ToString()))
                    .ToList();

            return View(request);
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
