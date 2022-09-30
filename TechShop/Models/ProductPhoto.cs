using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TechShop.Models
{
    public class ProductPhoto
    {
        public int Id { get; set; }
        [MaxLength(100)]

        public string Name { get; set; }
        [Range(1,int.MaxValue)]

        public int Order { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

    }
}
