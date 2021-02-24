using Microsoft.AspNet.Identity;
using Shopping_Site.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Shopping_Site.Controllers
{
    [Authorize(Roles = "User,Collaborator,Admin")]
    public class OrdersController : Controller
    {
        private Models.ApplicationDbContext db = new Models.ApplicationDbContext();
        //Arat Cosul de cumparaturi
        public ActionResult Index() // imi arada comanda curenta
        {
            string currentUser = User.Identity.GetUserId();
            //toate produsele din cos
            var products = db.Orders.Where(o => (o.UserID == currentUser && o.Shipping_Status != "Delivered"));
            if (products.Count() > 0)
                ViewBag.produseInCos = products.First().Products;
            return View();
        }

        public ActionResult Show(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Order ord = db.Orders.Find(id);
            return View(ord);
        }
        [HttpPost]
        public ActionResult AdaugaInCos(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string currentUser = User.Identity.GetUserId();
            var products = db.Orders.Where(o => (o.UserID == currentUser && o.Shipping_Status != "Delivered"));// determin daca utilizatorul are deja o comanda nefinalizata
            Order order;// creez o comanda noua
            if (products.Count() == 0) // daca utilizatorul nu are deja o comanda care nu e finalizata
            {
                order = new Order();
                order.UserID = User.Identity.GetUserId();
                order.Shipping_Status = "Produse in Cos";
                order = db.Orders.Add(order);
            }
            else// daca are o comanda care nu e finalizata
            {
                order = products.FirstOrDefault();
                foreach (var product in order.Products)
                {
                    if (product.ProductID == id)
                    {
                        product.Ammount = product.Ammount + 1;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
            }
            var prodcutToAdd = db.Products.Find(id);

            OrderProduct orderProduct = new OrderProduct();
            orderProduct.Ammount = 1;
            orderProduct.Price = prodcutToAdd.Price;
            orderProduct.OrderID = order.ID;
            orderProduct.ProductID = (int)id;
            db.OrderProducts.Add(orderProduct);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult finishOrder()
        {
            string currentUser = User.Identity.GetUserId();
            var orderToFinish = db.Orders.Where(o => (o.UserID == currentUser && o.Shipping_Status != "Delivered"));
            if (orderToFinish.Count() <= 0)
            {
                TempData["eroare"] = "Nu exita o comanda pentru finalizare";
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var product in orderToFinish.FirstOrDefault().Products)
                {
                    if (product.Ammount > product.Product.Count)
                    {
                        TempData["eroare"] = "Incercati sa cumparati un produs care nu este in stock";
                        return View();
                    }
                }
                ViewBag.orderID = orderToFinish.FirstOrDefault().ID;
            }
            
            return View();
        }

        [HttpPost]
        public ActionResult finishOrder(int id, string adress)
        {
            string currentUser = User.Identity.GetUserId();
            var orderToFinish = db.Orders.Find(id);
            if (orderToFinish == null)
            {
                TempData["eroare"] = "Nu exita o comanda pentru finalizare";
                return RedirectToAction("Index");
            }
            else
            {
                Order order = orderToFinish;
                order.Shipping_Address = adress;
                order.Order_Date = DateTime.Now;
                order.Shipping_Date = DateTime.Now.AddDays(7);
                order.Shipping_Status = "Delivered";
                foreach (var product in order.Products)
                {
                    product.Price = product.Product.Price;//setez pretul care era atunci cand am cumparat produsul
                    product.Product.Count = product.Product.Count - product.Ammount; // scad produsele din stock
                }
                db.SaveChanges();
            }

            return RedirectToAction("Index");

        }
        [HttpGet]
        public ActionResult allOrders()
        {
            string currentUser = User.Identity.GetUserId();
            var orders = db.Orders.Where(o => o.UserID == currentUser && o.Shipping_Status == "Delivered");
            return View(orders.ToList());
        }

    }
}