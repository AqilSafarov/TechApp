using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechShop.Models;

namespace TechShop.View_Models
{
    public class ProductVm
    {   
        public List<Slider> Sliders { get; set; }
        public List<Product> DiscountedProduct { get; set; }
        public List<Product> AllProduct { get; set; }
        public List<Product> TopRateProduct { get; set; }
        public List<ProductPhoto> ProductPhotos { get; set; }
        public List<Product> ProductHeadPhone { get; set; }
        public List<Product> Products { get; set; }
        public List<Category> Categories { get; set; }
        public List<Tag> Tags { get; set; }
        public Product Product { get; set; }
    }
}
