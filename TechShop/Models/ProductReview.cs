using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TechShop.Models
{
    public class ProductReview:BaseEntity
    {
        [MaxLength(50)]
        public string  Fullname { get; set; }
        [MaxLength(50)]

        public string  Email { get; set; }
        [MaxLength(500)]

        public string  Message { get; set; }
        [Range(1,5)]
        public int Rate { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
