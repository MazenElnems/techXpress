using System.Diagnostics;
using BusinessLogicLayer.Abstraction;
using BusinessLogicLayer.DTOs.Products;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Models;
using PresentationLayer.VMs.Category;
using PresentationLayer.VMs.Products;

namespace PresentationLayer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductManager _productManager;
        private readonly ICategoryManager _categoryManager;

        public HomeController(ILogger<HomeController> logger, IProductManager productManager , ICategoryManager categoryManager)
        {
            _logger = logger;
            _productManager = productManager;
            _categoryManager = categoryManager;
        }

        public IActionResult Index()
        {
            IEnumerable<CategoryVM> categories = _categoryManager.GetAll()
                .Select(c => c.ToVM());

            IEnumerable<ProductVM> products = _productManager.GetAll()
                .Select(p => p.ToProductVM())
                .ToList();

            return View(products);
        }

        public IActionResult Details(int id)
        {
            ProductDTO? productDTO = _productManager.GetById(id);

            if (productDTO != null)
            {
                return View(productDTO.ToProductVM());
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
