using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Shopping_Site.Models;

namespace Shopping_Site.Controllers
{
    public class ReviewsController : Controller
    {
        private Models.ApplicationDbContext db = new Models.ApplicationDbContext();

        [HttpPost]
        [Authorize(Roles ="User,Collaborator,Admin")]
        public ActionResult New(Review review)
        {
            review.UserID = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                review.Date = DateTime.Now;
                db.Reviews.Add(review);
                db.SaveChanges();
                return Redirect("/Products/Show/" + review.ProductID);
            }

            return Redirect("/Products/Show/" + review.ProductID);
        }

        [Authorize(Roles = "User,Collaborator,Admin")]
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
            if(review.UserID == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                return View(review);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui review care nu va apartine";
                return Redirect("/Products/Show/" + review.ProductID);
            }
           
        }

        [HttpPost]
        [Authorize(Roles ="User,Collaborator,Admin")]
        public ActionResult Edit(Review review)
        {
            try
            {
                Review reviewInDb = db.Reviews.Find(review.ID);
                if (reviewInDb.UserID == User.Identity.GetUserId() || User.IsInRole("Admin"))
                {
                    if (TryUpdateModel(reviewInDb))
                    {
                        reviewInDb.Rating = review.Rating;
                        reviewInDb.Comment = review.Comment;
                        db.SaveChanges();
                    }
                    return Redirect("/Products/Show/" + reviewInDb.ProductID);
                }
                else
                {
                    TempData["message"] = "Nu puteti edita review-ul altui utilizator!";
                    return Redirect("/Products/Show/" + reviewInDb.ProductID);
                }
            }
            catch (Exception e)
            {
                return View();
            }


        }

        [Authorize(Roles = "User,Collaborator,Admin")]
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
            if (review.UserID == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                return View(review);
            }
            else
            {
                TempData["message"] = "Nu puteti sterge review-ul altui utilizator!";
                return RedirectToAction("Index");
            }
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "User,Collaborator,Admin")]

        public ActionResult DeleteConfirmed(int id)
        {
            Review review = db.Reviews.Find(id);
            if (review.UserID == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                db.Reviews.Remove(review);
                db.SaveChanges();
                return Redirect("/Products");
            }
            else
            {
                TempData["message"] = "Nu puteti sterge review-ul altui utilizator!";
                return RedirectToAction("Index");
            }
        }

    }
}
