using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Shopping_Site.Models;

namespace Shopping_Site.Controllers
{
    public class ProductsController : Controller
    {
        private Models.AppContext db = new Models.AppContext();

        // GET: Product
        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            return View(db.Products.Where(p=>p.IsApproved).ToList());
        }

        // GET: Product/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Product/Create
        public ActionResult Create()
        {
            ViewBag.Categorii = db.Categories;
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Description,Count,Price,CategoryId")] Product product)
        {
            product.IsApproved = false;
            product.RegisteredOn = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                TempData["message"] = "Produsul asteapta sa fie validat!";
                return RedirectToAction("Index");
            }

            return View(product);
        }

        // GET: Product/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            ViewBag.Categorii = db.Categories;
            if (product == null)
            {
                return HttpNotFound();
            }
            TempData["message"] = "Produsul a fost editat!";
            return View(product);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product)
        {
            try
            {
                Product prod = db.Products.Find(product.ID);
                if(TryUpdateModel(prod))
                {
                    prod.Name = product.Name;
                    prod.Price = product.Price;
                    prod.IsApproved = product.IsApproved;
                    prod.Description = product.Description;
                    prod.Count = product.Count;
                    prod.CategoryId = product.CategoryId;
                    db.SaveChanges();
                }
                TempData["message"] = "Produsul a fost editat!";
                return Redirect("/Products/Details/" + prod.ID);
            }
            catch (Exception e)
            {
                return View();
            }
        }

        // GET: Product/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            TempData["message"] = "Produsul a fost sters!";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Valideaza()
        {
            TempData["message"] = "Produsul a fost validat!";
            return View(db.Products.Where(p=>p.IsApproved!=true).ToList());
        }
        [HttpPost]
        public ActionResult Valideaza(int? id)
        {
            if (id == null)
                return null;
            var prod = db.Products.Find(id);
            prod.IsApproved = true;
            db.SaveChanges();
            return RedirectToAction("Valideaza");
        }

    }
}
