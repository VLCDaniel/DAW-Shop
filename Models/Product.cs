using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Shopping_Site.Models
{
    public class Product
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        public DateTime RegisteredOn { get; set; }
        public string Description { get; set; }
        public int Count { get; set; }
        public bool IsApproved { get; set; }
        public int Price { get; set; }

        [Required(ErrorMessage = "Categoria este obligatorie")]
        public int CategoryId { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }
    }
}