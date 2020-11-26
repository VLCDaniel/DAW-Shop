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
    public class ReviewsController : Controller
    {
        private Models.AppContext db = new Models.AppContext();

        [HttpPost]
        public ActionResult New(Review review)
        {
            if (ModelState.IsValid)
            {
                review.Date = DateTime.Now;
                db.Reviews.Add(review);
                db.SaveChanges();
                return Redirect("/Products/Show/" + review.ProductID);
            }

            return Redirect("/Products/Show/" + review.ProductID);
        }

        // GET: Reviews/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        [HttpPost]
        public ActionResult Edit(Review review)
        {
            try
            {
                Review comm = db.Reviews.Find(review.ID);
                if (TryUpdateModel(comm))
                {
                    comm.Comment = review.Comment;
                    db.SaveChanges();
                }
                return Redirect("/Products/Show/" + comm.ProductID);
            }
            catch (Exception e)
            {
                return View();
            }


        }

        // GET: Reviews/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        
        public ActionResult DeleteConfirmed(int id)
        {
            Review review = db.Reviews.Find(id);
            db.Reviews.Remove(review);
            db.SaveChanges();
            return Redirect("/Products");
        }

    }
}
