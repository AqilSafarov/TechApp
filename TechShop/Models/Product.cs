using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TechShop.Models
{
    public class Product:BaseEntity
    {

        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(120)]

        public string Slug { get; set; }
        [Column(TypeName= "float"),Range(0,double.MaxValue)]
        public double Price { get; set; }
        [Column(TypeName = "float"), Range(0, double.MaxValue)]

        public double ProducingPrice { get; set; }

        [Column(TypeName = "float"), Range(0, double.MaxValue)]

        public double DiscountPrice { get; set; }
        [MaxLength(1500)]

        public string Desc { get; set; }
        [MaxLength(500)]

        public string InfoText { get; set; }

        [Column(TypeName = "bit")]
        public bool IsAvailable { get; set; }
        [Column(TypeName = "float")]
        public double Rate { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public List<ProductTag> ProductTags { get; set; }

        [NotMapped]
        public int[] TagIds { get; set; }

        public List<ProductPhoto> ProductPhotos { get; set; }

        [NotMapped]
        public List<IFormFile> Files { get; set; } = new List<IFormFile>();

        [NotMapped]
        public List<int> FileIds { get; set; }

    }

}
