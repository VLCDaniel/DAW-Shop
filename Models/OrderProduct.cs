using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Shopping_Site.Models
{
    public class OrderProduct
    {
        [Key]
        public int ID { get; set; }
        public int Ammount { get; set; }
        public float Price { get; set; }

        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}