using BulkyBookWeb.Data;
using BulkyBookWeb.Models;
using Microsoft.AspNetCore.Authorization;
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


            if (Request.Cookies["Message"] != null)
            {
                var messages = JObject.Parse(Request.Cookies["Message"]);
                Message = $"{messages["Message"]}";
                MessageType = $"{messages["MessageType"]}";
                Response.Cookies.Delete("Message");
            }

            List<Category> objCategoryList = _db.Categories.ToList();
            Tuple<List<Category>, String, String, String> ViewContext = new Tuple<List<Category>, String, String, String>(objCategoryList, "Categories", Message, MessageType);

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
                var dict = new Hashtable();
                dict["MessageType"] = "success";
                dict["Message"] = $"One Category Added Successfully.";
                var sessiondata = JsonConvert.SerializeObject(dict);
                Response.Cookies.Append("Message", sessiondata, cookieOptions);

                DateTime curDateTime = DateTime.Now;
                obj.CreatedDateTime = curDateTime;
                _db.Categories.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Index", "Category");
            }
            return View(obj);
        }


        public IActionResult Edit(int id)
        {
            Category obj = _db.Categories.FirstOrDefault(x => x.Id == id);
            if (obj == null)
            {
                var dict = new Hashtable();
                dict["MessageType"] = "fail";
                dict["Message"] = $"Element not found on Id : {id}, Edit Request Denied!";
                var sessiondata = JsonConvert.SerializeObject(dict);
                Response.Cookies.Append("Message", sessiondata, cookieOptions);
                return RedirectToAction("Index", "Category");
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category obj)
        {
            if (obj.Name != null && !Regex.IsMatch(obj.Name, @"^[a-zA-Z]+$"))
            {
                ModelState.AddModelError("Name", "Name can't have digits or special characters.");
            }
            if (ModelState.IsValid)
            {
                var dict = new Hashtable();
                dict["MessageType"] = "success";
                dict["Message"] = $"One Category Updated Successfully on Id : {obj.Id}.";
                var sessiondata = JsonConvert.SerializeObject(dict);
                Response.Cookies.Append("Message", sessiondata, cookieOptions);

                DateTime curDateTime = DateTime.Now;
                obj.CreatedDateTime = curDateTime;
                _db.Categories.Update(obj);
                _db.SaveChanges();
                return RedirectToAction("Index", "Category");
            }
            return View(obj);
        }

        public IActionResult Delete(int id)
        {
            var dict = new Hashtable();
            String sessiondata;
            Category categoryToDelete = _db.Categories.FirstOrDefault(x => x.Id == id);
            if (categoryToDelete == null)
            {
                
                dict["MessageType"] = "fail";
                dict["Message"] = $"Element not found on Id : {id}, Deletion Failed!";
                sessiondata = JsonConvert.SerializeObject(dict);
                Response.Cookies.Append("Message", sessiondata, cookieOptions);
                return RedirectToAction("Index", "Category");
            }

            _db.Categories.Remove(categoryToDelete);
            _db.SaveChanges();
            
            dict["MessageType"] = "success";
            dict["Message"] = $"One Category Removed Successfully.";
            sessiondata = JsonConvert.SerializeObject(dict);
            Response.Cookies.Append("Message", sessiondata, cookieOptions);
            return RedirectToAction("Index", "Category");
        }
    }
}
