using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Shopping_Site.Models;

namespace Shopping_Site.Controllers
{
    public class OrderProductsController : Controller
    {
        private Models.ApplicationDbContext db = new Models.ApplicationDbContext();

        // GET: OrderProducts
        public ActionResult Index()
        {
            return Redirect("Orders");
        }
        [Authorize(Roles = "User,Collaborator,Admin")]
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            OrderProduct orderProduct = db.OrderProducts.Find(id);
            if(orderProduct.Order.UserID == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                db.OrderProducts.Remove(orderProduct);
                db.SaveChanges();
            }
            

            return RedirectToAction("Index");
        }
    }
}