using BusinessLogicLayer.Abstraction;
using BusinessLogicLayer.DTOs;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.ActionRequests;
using PresentationLayer.VMs;

namespace PresentationLayer.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductManager _productManager;
        private readonly IFilesService _filesService;
        public ProductController(IProductManager productManager, IFilesService filesService)
        {
            _productManager = productManager;
            _filesService = filesService;
        }

        [HttpPost]
        public IActionResult Index(string searchTerm, string searchBy, decimal? minPrice, decimal? maxPrice)
        {
            IEnumerable<ProductVM> productVMs = _productManager
                .GetProductsByFilter(searchTerm,searchBy,minPrice,maxPrice)
                .Select(p => new ProductVM
                {
                    Description = p.Description,
                    Image = p.Image,
                    Name = p.Name,
                    Price = p.Price
                });

            ViewData["SearchTerm"] = searchTerm;
            ViewData["SearchBy"] = searchBy;
            ViewData["MinPrice"] = minPrice;
            ViewData["MaxPrice"] = maxPrice;

            return View(productVMs);
        }

        [HttpGet]
        public IActionResult Index()
        {
            IEnumerable<ProductVM> productVMs = _productManager
                .GetAll()
                .Select(p => new ProductVM
                {
                    Description = p.Description,
                    Image = p.Image,
                    Name = p.Name,
                    Price = p.Price
                });

            return View(productVMs);
        }

        [HttpGet]
        public IActionResult Create()
        {
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
                Price = productActionRequest.Price
            };

            _productManager.Create(productDTO);

            return RedirectToAction(nameof(Index));
        }
    }
}
