using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechShop.Models;

namespace TechShop.View_Models
{
    public class ContactVm
    {
        public MessageContact MessageContact { get; set; }
        public Setting Setting { get; set; }
        public List<Product> Products { get; set; }
    }
}
