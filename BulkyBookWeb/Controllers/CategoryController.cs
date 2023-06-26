using Azure;
using Azure.Core;
using BulkyBookWeb.Data;
using BulkyBookWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Net;
using System.Text.RegularExpressions;

namespace BulkyBookWeb.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly AuthDBContext _db;
        CookieOptions cookieOptions = new CookieOptions
        {
            Secure = true,
            HttpOnly = true,
            SameSite = SameSiteMode.None
        };
        public CategoryController(AuthDBContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {

            String Message = "";
            String MessageType = "success";


           

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
            if (obj.Name != null && !Regex.IsMatch(obj.Name, @"^[a-zA-Z]+$"))
            {
                ModelState.AddModelError("Name", "Name can't have digits or special characters.");
            }
            if (ModelState.IsValid)
            {
                
                TempData["MessageType"] = "success";
                TempData["Message"] = $"One Category Added Successfully.";
                

                DateTime curDateTime = DateTime.Now;
                obj.CreatedDateTime = curDateTime;
                _db.Categories.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index", "Category");
            }
            return View(obj);
        }


        public IActionResult Edit(int? id)
        {
            if(id != null) {
                NotFound();
            }
            Category obj = _db.Categories.FirstOrDefault(x => x.Id == id);
            if (obj == null)
            {
                
                TempData["MessageType"] = "fail";
                TempData["Message"] = $"Element not found on Id : {id}, Edit Request Denied!";
                
                return RedirectToAction("Index", "Category");
            }
            return View(obj);
        }

        [HttpPost,ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult EditPOST(Category obj)
        {
            if (obj.Name != null && !Regex.IsMatch(obj.Name, @"^[a-zA-Z]+$"))
            {
                ModelState.AddModelError("Name", "Name can't have digits or special characters.");
            }
            if (ModelState.IsValid)
            {
                
                TempData["MessageType"] = "success";
                TempData["Message"] = $"One Category Updated Successfully on Id : {obj.Id}.";
                

                DateTime curDateTime = DateTime.Now;
                obj.CreatedDateTime = curDateTime;
                _db.Categories.Update(obj);
                _db.SaveChanges();
                return RedirectToAction("Index", "Category");
            }
            return View(obj);
        }

		[ActionName("Delete")]
		public IActionResult Delete_(int id)
        {
            Category categoryToDelete = _db.Categories.FirstOrDefault(x => x.Id == id);
            if (categoryToDelete == null)
            {
                
                TempData["MessageType"] = "fail";
                TempData["Message"] = $"Element not found on Id : {id}, Deletion Failed!";
                
                return RedirectToAction("Index", "Category");
            }

            _db.Categories.Remove(categoryToDelete);
            _db.SaveChanges();
            TempData["Message"] = $"One Category Removed Successfully.";
            TempData["MessageType"] = "success";

			return RedirectToAction("Index", "Category");
        }
    }
}


/*
// Add data to cookies:
dict["MessageType"] = "success";
dict["Message"] = $"One Category Removed Successfully.";
sessiondata = JsonConvert.SerializeObject(dict);
Response.Cookies.Append("Message", sessiondata, cookieOptions);


#################

// Read data from Cookies:
if (Request.Cookies["Message"] != null)
{
	var messages = JObject.Parse(Request.Cookies["Message"]);
	Message = $"{messages["Message"]}";
	MessageType = $"{messages["MessageType"]}";
	
}

################

// delete Cookies

Response.Cookies.Delete("Message");

 */

