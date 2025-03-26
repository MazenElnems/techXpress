using System.Diagnostics;
using BusinessLogicLayer.Abstraction;
using BusinessLogicLayer.DTOs.Products;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Models;
using PresentationLayer.VMs.Products;

namespace PresentationLayer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductManager _productManager;

        public HomeController(ILogger<HomeController> logger, IProductManager productManager)
        {
            _logger = logger;
            _productManager = productManager;
        }

        public IActionResult Index()
        {
            IEnumerable<ProductVM>? products = _productManager
                .GetAll()
                .Select(p => p.ToProductVM());

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
