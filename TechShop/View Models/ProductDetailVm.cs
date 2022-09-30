using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechShop.Models;

namespace TechShop.View_Models
{
    public class ProductDetailVm
    {
        public Product Product { get; set; }
        public List<Product> RelatedProducts { get; set; }
    }
}
