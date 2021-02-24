using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Shopping_Site.Models;

namespace Shopping_Site.Controllers
{
    public class ProductsController : Controller
    {
        private Models.ApplicationDbContext db = new Models.ApplicationDbContext();
        private int _perPage = 9;

        public ActionResult Index()
        {
            var products = db.Products.Where(p => p.IsApproved).Include("User").OrderBy(p => p.RegisteredOn);
            var search = "";

            if (Request.Params.Get("filter") == "1")
            {
                products = db.Products.Include("User").OrderBy(p => p.Price);
                ViewBag.Selected = "selected";
                ViewBag.FilterString = "1";
            }

            if (Request.Params.Get("filter") == "2")
            {
                products = db.Products.OrderBy(p => (-1) * p.Price);
                ViewBag.Selected = "selected";
                ViewBag.FilterString = "2";
            }

            if (Request.Params.Get("filter") == "3")
            {
                products = db.Products.OrderBy(p => (-1) * p.Price);
                ViewBag.Selected = "selected";
                ViewBag.FilterString = "3";
            }

            if (Request.Params.Get("filter") == "4")
            {
                products = db.Products.OrderBy(p => (-1) * p.Price);
                ViewBag.Selected = "selected";
                ViewBag.FilterString = "4";
            }

            if (Request.Params.Get("search") != null)
            {
                search = Request.Params.Get("search").Trim(); // trim whitespace from search string
                // Search in products (title and description)
                List<int> productIds = db.Products.Where(
                    pd => pd.Name.Contains(search)
                    || pd.Description.Contains(search)
                    ).Select(p => p.ID).ToList();

                // Search in comments (content)
                List<int> commentIds = db.Reviews.Where(r => r.Comment.Contains(search)).Select(com => com.ProductID).ToList();

                // Unique list of products
                List<int> mergedIds = productIds.Union(commentIds).ToList();

                // List of products that contain the search string either in product title, description or comment
                if (Request.Params.Get("filter") == "1")
                {
                    products = db.Products.Where(product => mergedIds.Contains(product.ID)).Include("User").OrderBy(p => p.Price);
                    ViewBag.Selected1 = "selected";
                    ViewBag.FilterString = "1";
                }
                else if (Request.Params.Get("filter") == "2")
                {
                    products = db.Products.Where(product => mergedIds.Contains(product.ID)).Include("User").OrderBy(p => (-1)*p.Price);
                    ViewBag.Selected2 = "selected";
                    ViewBag.FilterString = "2";
                }
                else if (Request.Params.Get("filter") == "3")
                {
                    var products2 = db.Products.Where(product => mergedIds.Contains(product.ID)).Include("User").ToList();
                    products2 = products2.OrderBy(p => getStars(p)).ToList();
                    products = (IOrderedQueryable<Product>)products2.AsQueryable();
                    ViewBag.Selected3 = "selected";
                    ViewBag.FilterString = "3";
                }
                else if (Request.Params.Get("filter") == "4")
                {
                    var products2 = db.Products.Where(product => mergedIds.Contains(product.ID)).Include("User").ToList();
                    products2 = products2.OrderByDescending(p => getStars(p)).ToList();
                    
                    products = (IOrderedQueryable<Product>)products2.AsQueryable();
                    ViewBag.Selected4 = "selected";
                    ViewBag.FilterString = "4";
                }
                else
                {
                    products = db.Products.Where(product => mergedIds.Contains(product.ID)).Include("User").OrderBy(p => p.RegisteredOn);
                }
            }

            var totalProducts = products.Count();
            var currentPage = Convert.ToInt32(Request.Params.Get("page"));

            var offset = 0;

            if(!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * this._perPage;
            }

            var paginatedProducts = products.Skip(offset).Take(this._perPage).Where(p => p.IsApproved);

            ViewBag.total = totalProducts;
            ViewBag.lastPage = Math.Ceiling((float)totalProducts / (float)this._perPage);
            ViewBag.Products = paginatedProducts.Where(p => p.IsApproved);

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            ViewBag.afisareButoane = false;
            if (User.IsInRole("Collaborator") || User.IsInRole("Admin"))
            {
                ViewBag.afisareButoane = true;
            }

            ViewBag.esteAdmin = User.IsInRole("Admin");
            ViewBag.utilizatorCurent = User.Identity.GetUserId();
            ViewBag.SearchString = search;
            return View(db.Products.Where(p => p.IsApproved).Include("User").ToList());
        }

        public ActionResult Show(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);

            ViewBag.afisareButoane = false;
            if (User.IsInRole("Collaborator") || User.IsInRole("Admin"))
            {
                ViewBag.afisareButoane = true;
            }

            ViewBag.esteAdmin = User.IsInRole("Admin");
            ViewBag.utilizatorCurent = User.Identity.GetUserId();


            if (product == null)
            {
                return HttpNotFound();
            }
            decimal reviewsSum = 0;
            var reviews = db.Reviews.Where(e => e.ProductID == id);
            if (reviews.Count() == 0)
                return View(product);
            foreach (var review in reviews)
            {
                reviewsSum += review.Rating;
            }

            ViewBag.rating = Decimal.Divide(reviewsSum, reviews.Count());
            return View(product);
        }

        [Authorize(Roles = "Collaborator,Admin")]

        public ActionResult New()
        {
            ViewBag.Categorii = db.Categories;
            return View();
        }

        [Authorize(Roles = "Collaborator,Admin")]
        [HttpPost]
        public ActionResult New(Product product)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categorii = db.Categories;
                return View(product);
            }

            product.UserID = User.Identity.GetUserId();
            product.IsApproved = false;
            product.RegisteredOn = DateTime.Now;
            HttpPostedFileBase uploadedFile = product.UploadedFile;
            product.FileName = uploadedFile.FileName;

            var uploadedFileExtension = Path.GetExtension(uploadedFile.FileName);

            if (uploadedFileExtension == ".png" || uploadedFileExtension == ".jpg" || uploadedFileExtension == ".pdf")
            {

                // Se seteaza calea folderului de upload
                string uploadFolderPath = Server.MapPath("~//Files//");

                // Se salveaza fisierul in acel folder
                uploadedFile.SaveAs(uploadFolderPath + uploadedFile.FileName);

            }

            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                TempData["message"] = "Produsul asteapta sa fie validat!";
                return RedirectToAction("Index");
            }
            return View(product);
        }

        [Authorize(Roles = "Collaborator,Admin")]
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

            if (product.UserID == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                return View(product);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui articol care nu va apartine";
                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        [Authorize(Roles = "Collaborator,Admin")]
        public ActionResult Edit(Product product)
        {
            try
            {
                Product prod = db.Products.Find(product.ID);
                if (prod.UserID == User.Identity.GetUserId() || User.IsInRole("Admin"))
                {

                    if (TryUpdateModel(prod))
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
                    return Redirect("/Products/Show/" + prod.ID);
                }
                else
                {
                    TempData["message"] = "Nu puteti edita produsul altui utilizator!";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                return View();
            }
        }

        [Authorize(Roles = "Collaborator,Admin")]
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

            if (product.UserID == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                return View(product);
            }
            else
            {
                TempData["message"] = "Nu puteti sterge produsul altui utilizator!";
                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "Collaborator,Admin")]
        [HttpDelete, ActionName("Delete")]
        public ActionResult Delete(int id)
        {
            Product product = db.Products.Find(id);
            if (product.UserID == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {   
                if(db.OrderProducts.Where(o=>o.ProductID == id).Count()>0)//daca exista o comanda facuta cu acel produs nu il putem sterge
                {
                    product.IsApproved = false; //deci il facem neaprobat
                }
                else
                {
                    string strPhysicalFolder = Server.MapPath("~/Files/");

                    string strFileFullPath = strPhysicalFolder +product.FileName;

                    if (System.IO.File.Exists(strFileFullPath))
                    {
                        System.IO.File.Delete(strFileFullPath);
                    }

                    db.Products.Remove(product);
                    db.SaveChanges();
                    TempData["message"] = "Produsul a fost sters!";
                }
                
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu puteti sterge produsul altui utilizator!";
                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Valideaza()
        {

            return View(db.Products.Where(p => p.IsApproved != true).ToList());
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public ActionResult Valideaza(int? id)
        {
            if (id == null)
                return null;

            Product prod = db.Products.Find(id);

            prod.IsApproved = true;
            db.SaveChanges();

            TempData["message"] = "Produsul a fost validat cu succes!";

            return RedirectToAction("Index");
        }
        [NonAction]
        public int getStars(Product p)
        {
            var reviewsSum = 0;
            var reviews = db.Reviews.Where(e => e.ProductID == p.ID);
            if (reviews.Count() == 0)
                return 0;
            foreach (var review in reviews)
            {
                reviewsSum += review.Rating;
            }
            return reviewsSum;
        }
    }
}
