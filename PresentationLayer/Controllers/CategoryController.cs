using BusinessLogicLayer.Abstraction;
using BusinessLogicLayer.Managers;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.VMs.Category;

namespace PresentationLayer.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryManager _categoryManager;
        public CategoryController(ICategoryManager categoryManager)
        {
            _categoryManager = categoryManager;
        }

        public IActionResult Index()
        {
            IEnumerable<CategoryVM> categoryVMs = _categoryManager.GetAll()
                .Select(c => c.ToVM());

            return View(categoryVMs);
        }

    }
}
