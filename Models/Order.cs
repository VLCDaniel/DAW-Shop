using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Shopping_Site.Models
{
    public class Order
    {
        [Key]
        public int ID { get; set; }
        //public int User_id { get; set; }
        public string Shipping_Address { get; set; }
        public DateTime Order_Date { get; set; }
        public DateTime Shipping_Date { get; set; }
        public string Shipping_Status { get; set; }
    }
}