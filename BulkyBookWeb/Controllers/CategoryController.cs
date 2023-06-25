using BulkyBookWeb.Data;
using BulkyBookWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Text.RegularExpressions;

namespace BulkyBookWeb.Controllers
{
    //[Authorize]
    public class CategoryController : Controller
    {
        private readonly AuthDBContext _db;
        public CategoryController(AuthDBContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            List<Category> objCategoryList = _db.Categories.ToList();
            Tuple<List<Category>, String> ViewContext = new Tuple<List<Category>, String>(objCategoryList, "Categories");

            return View(ViewContext);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category obj)
        {
            if (obj.Name!=null && !Regex.IsMatch(obj.Name, @"^[a-zA-Z]+$"))
            {
                ModelState.AddModelError("Name", "Name can't have digits or special characters.");
            }
            if (ModelState.IsValid) {
                DateTime curDateTime = DateTime.Now;
                obj.CreatedDateTime = curDateTime;
                _db.Categories.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index", "Category");
            }
            return View(obj);
        }
    }
}
