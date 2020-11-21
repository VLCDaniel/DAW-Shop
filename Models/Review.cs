using System;
using System.ComponentModel.DataAnnotations;

namespace Shopping_Site.Models
{
    public class Review
    {
        [Key]
        public int ID { get; set; }
        [Range(0,5)][Required]
        public short Rating { get; set; }
        [Required]
        public string Comment { get; set; }

        public int ProductID { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public virtual Product Product { get; set; }
    }
}