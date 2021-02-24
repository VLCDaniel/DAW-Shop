using System;
using System.ComponentModel.DataAnnotations;

namespace Shopping_Site.Models
{
    public class Review
    {
        [Key]
        public int ID { get; set; }
        [Range(0,5)][Required(ErrorMessage = "Rating-ul produsului este obligatoriu!")]
        public short Rating { get; set; }
        [Required(ErrorMessage = "Comentariul este obligatoriu!")]
        public string Comment { get; set; }

        public int ProductID { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public virtual Product Product { get; set; }

        public virtual ApplicationUser User { get; set; }
        public string UserID { get; set; }
    }
}