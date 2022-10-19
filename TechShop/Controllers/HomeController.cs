using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TechShop.Models;
using TechShop.View_Models;

namespace TechShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
           List<Product> product = await _context.Products.Include(x => x.ProductPhotos)
              .Include(x => x.Category)
              .Include(x => x.ProductReviews)
              .Include(x => x.ProductTags)
              .ThenInclude(x => x.Tag).ToListAsync();

            List<Product> productHead = await _context.Products.Where(x => x.CategoryId == 3).Include(x => x.ProductPhotos)
             .Include(x => x.Category)
             .Include(x => x.ProductReviews)
             .Include(x => x.ProductTags)
             .ThenInclude(x => x.Tag).ToListAsync();

            ProductVm productVm = new ProductVm
            {
                Sliders = await _context.Sliders.Take(3).ToListAsync(),
                AllProduct = await _context.Products.Include(x => x.ProductPhotos).Include(x => x.ProductReviews).OrderByDescending(x => x.CreatedAt).Take(10).ToListAsync(), //Reviwe qaldi
                TopRateProduct = await _context.Products.Where(x => x.Rate > 3).Include(x => x.ProductPhotos).OrderByDescending(x => x.CreatedAt).Take(12).ToListAsync(),
                DiscountedProduct = await _context.Products.Where(x => x.DiscountPercent >0).OrderByDescending(x => x.CreatedAt).Take(12).ToListAsync(),
                Products = product,
                ProductHeadPhone= productHead,


            };
            return View(productVm);
        }
        [HttpPost]

      
        public IActionResult Subscribe(Subscribe subcribe)
        {
            if (ModelState.IsValid)
            {
                
                if (!_context.Subscribes.Any(x=>x.Email==subcribe.Email))
                {
                    subcribe.CreatedAt = DateTime.UtcNow;
                    _context.Subscribes.Add(subcribe);
                    _context.SaveChanges();
                }
                ModelState.AddModelError("Email", "Artiq abune olmusuz");

            }

            return RedirectToAction("Index","Home");

        }



    }
}
