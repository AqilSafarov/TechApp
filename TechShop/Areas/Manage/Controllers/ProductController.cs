using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechShop.Areas.Manage.ViewModels;
using TechShop.Models;

namespace TechShop.Areas.Manage.Controllers
{
    [Area("manage")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int page=1)
        {

            double totalCount = await _context.Categories.CountAsync();
            int pageCount = (int)Math.Ceiling(totalCount / 2);

            if (page < 1) page = 1;
            else if (page > pageCount) page = pageCount;

            ViewBag.PageCount = pageCount;
            ViewBag.SelectedPage = page;
            ProductVm productVm = new ProductVm
            {
                Products = await _context.Products.Include(x=>x.Category).ToListAsync()
        };
            return View(productVm);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.Where(x => !x.IsDeleted).ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            #region ModelState
            if (!ModelState.IsValid)
            {
                return View();
            }
            #endregion

            #region CheheckSlug
            if (await _context.Products.AnyAsync(x => x.Slug.ToLower() == product.Slug.Trim().ToLower()))
            {
                ModelState.AddModelError("Slug", "Bele bir mehsul artiq movcutdur");
                return View();

            }

            #endregion

            #region ChehckCategory

            if (!await _context.Categories.AnyAsync(x=>x.Id==product.CategoryId && !x.IsDeleted))  //slinememis ve gonderilmis id li data yoxdursa
            {
                ModelState.AddModelError("CategoryId", "Bele bir Category yoxdur");
                return View();
            }

            product.CreatedAt = DateTime.UtcNow;
            product.ModifideAt = DateTime.UtcNow;
            product.DiscountPrice = product.DiscountPrice <= 0 ? product.Price : (product.Price * (100 - product.DiscountPrice) / 100);


            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            #endregion


            return RedirectToAction("Index");

        }
        public async Task<IActionResult> Edit()
        {
            return View();

        }
    }
}
