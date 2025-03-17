using BusinessLogicLayer.Abstraction;
using BusinessLogicLayer.DTOs.CategoryDTOs;
using BusinessLogicLayer.Managers;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.ActionRequests;
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

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateCategoryActionRequest request) 
        {
            if(ModelState.IsValid)
            {
                _categoryManager.Create(request.ToDto());
                TempData["successNotification"] = "Category Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            CategoryDTO? categoryDTO = _categoryManager.GetById(id);

            if(categoryDTO != null)
            {
                return View(categoryDTO.ToVM());
            }
            return RedirectToAction(nameof(Index));
        } 

        [HttpPost]
        public IActionResult Update(int id, UpdateCategoryActionRequest request)
        {
            CategoryDTO? categoryDTO = _categoryManager.GetById(id);

            if (categoryDTO != null) 
            {
                categoryDTO.Name = request.Name;
                _categoryManager.Update(categoryDTO);
                TempData["successNotification"] = "Category Updated Successfully";
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id) 
        {
            CategoryDTO? categoryDTO = _categoryManager.GetById(id);

            if (categoryDTO != null)
            {
                _categoryManager.Delete(id);
                TempData["successNotification"] = "Category Deleted Successfully";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
