using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TechShop.Models
{
    public class Category:BaseEntity
    {
        [MaxLength(50)]
        public string Name { get; set; }
        [Range(minimum:1,maximum:int.MaxValue)]
        public int Order { get; set; }
        [Column(TypeName ="bit")]
        public bool IsDeleted { get; set; }

        public List<Product> Products { get; set; }
    }
}
