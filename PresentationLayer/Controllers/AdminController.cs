using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShoppingStore.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Dashboard()
        {
            return View();
        }
        // GET: Admin/Categories
        public ActionResult Categories()
        {
            // Get all categories from repository
            List<Tbl_Category> allCategories = _unitOfWork.GetRepositoryInstance<Tbl_Category>().GetAllRecordsIQueryable().Where(i=>i.IsDelete==false)  
              .ToList();

            return View(allCategories);  // Pass categories to view
        }

    }
}
