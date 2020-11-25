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
        [Required(ErrorMessage = "Numele este obligatoriu!")]
        public string Name { get; set; }
        public DateTime RegisteredOn { get; set; }
        [Required(ErrorMessage = "Descrierea produsului este obligatorie!")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Numarul produselor este obligatoriu!")]
        public int Count { get; set; }
        public bool IsApproved { get; set; }
        [Required(ErrorMessage = "Pretul este obligatoriu!")]
        public int Price { get; set; }

        [Required(ErrorMessage = "Categoria este obligatorie")]
        public int CategoryId { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }
    }
}