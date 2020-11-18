using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shopping_Site.Controllers
{
    public class CategoriesController : Controller
    {
        private Models.AppContext db = new Models.AppContext();

        // GET: Categories
        public ActionResult Index()
        {
            var categories = db.Categories;
            ViewBag.Categories = categories;
            if(TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            return View();
        }

        // GET: Show
        public ActionResult Show(int id)
        {
            Models.Category category = db.Categories.Find(id);
            return View(category);
        }

        // GET: New
        public ActionResult New()
        {
            Models.Category category = new Models.Category();
            return View(category);
        }

        // POST: New
        [HttpPost]
        public ActionResult New(Models.Category category)
        {
            try
            {
                db.Categories.Add(category);
                db.SaveChanges();
                TempData["message"] = "Categoria a fost adaugata!";
                return RedirectToAction("Index");
            }

            catch(Exception e)
            {
                return View();
            }
        }

        // GET - Edit
        public ActionResult Edit(int id)
        {
            Models.Category category = db.Categories.Find(id);

            return View(category);
        }

        // PUT - Edit
        [HttpPut]
        public ActionResult Edit(int id, Models.Category requestCategory)
        {
            try
            {
                Models.Category category = db.Categories.Find(id);
                if(TryUpdateModel(category))
                {
                    category.CategoryName = requestCategory.CategoryName;
                    db.SaveChanges();
                    TempData["message"] = "Categoria a fost editata cu succes!";
                    return RedirectToAction("INdex");
                }
                return View(requestCategory);
            }

            catch (Exception e)
            {
                return View(requestCategory);
            }
        }

        // DELETE - Delete
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Models.Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            db.SaveChanges();
            TempData["message"] = "Categoria a fost stearsa cu succes!";
            return RedirectToAction("Index");
        }
    }
}