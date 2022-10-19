using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechShop.Models;
using TechShop.View_Models;

namespace TechShop.Services
{
    public class LayoutVmService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public LayoutVmService(AppDbContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }
        public List<Category> GetCategories()
        {
            List<Category> categories = _context.Categories.ToList();
            return categories;
        }
      
        public VmNavSetCat GetCategory()
        {
            VmNavSetCat vm = new VmNavSetCat()
            {
                CategoryPhone = _context.Categories.Where(x => x.Name == "Phone").FirstOrDefault(),
                CategoryWatch = _context.Categories.Where(x => x.Name == "Smartwatch").FirstOrDefault(),
                Setting = _context.Settings.FirstOrDefault()
            };
            return vm;
        }

        public VmBasketCard GetBasket()
        {
            VmBasketCard basketVm = new VmBasketCard();
            var basket = _contextAccessor.HttpContext.Request.Cookies["basket"];
            List<BasketCardItemModel> basketCardItemsModel = new List<BasketCardItemModel>();


            if (basket != null)
            {
                basketCardItemsModel = JsonConvert.DeserializeObject<List<BasketCardItemModel>>(basket);
            }

            foreach (var basketItem in basketCardItemsModel)
            {
                Product book = _context.Products.Include(x => x.ProductPhotos).FirstOrDefault(x => x.Id == basketItem.Id);

                if (book == null)
                    continue;
                BasketBookItemViewModel basketItemVm = new BasketBookItemViewModel
                {
                    Id = basketItem.Id,
                    Count = basketItem.Count,
                    Name = book.Name,
                    Price = book.DiscountPrice,
                    Poster = book.ProductPhotos.FirstOrDefault(x => x.Order == 1)?.Name,
                };
                basketVm.TotalPrice += book.DiscountPrice * basketItem.Count;
                basketVm.BasketBookItems.Add(basketItemVm);

            }

            return basketVm;
        }

    }
}
