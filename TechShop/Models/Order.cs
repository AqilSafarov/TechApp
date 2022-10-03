using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechShop.Helpers;

namespace TechShop.Models
{
    public class Order:BaseEntity
    {
        public int Count { get; set; }
        public double Price { get; set; }
        public double DiscountPrice { get; set; }
        public OrderStatus Status { get; set; }

        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }


    }
}
