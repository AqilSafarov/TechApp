using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechShop.View_Models
{
    public class VmBasketCard
    {
        public VmBasketCard()
        {
            BasketBookItems = new List<BasketBookItemViewModel>();
        }
        public List<BasketBookItemViewModel> BasketBookItems { get; set; }
        public double TotalPrice { get; set; }

    }
    public class BasketBookItemViewModel
    {
        public int Id { get; set; }
        public string Poster { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Count { get; set; }
    }
    public class BasketCardItemModel
    {
        public int Id { get; set; }
        public int Count { get; set; }
    }
}

