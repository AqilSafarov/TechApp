using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechShop.Models;
using TechShop.View_Models;

namespace TechShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }
        public async Task<IActionResult> Detail(int id)
        {
            Product product = await _context.Products.Include(x=>x.ProductPhotos)
                .Include(x=>x.Category)
                .Include(x=>x.ProductTags)
                .ThenInclude(x=>x.Tag).FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
                return NotFound();


            ProductDetailVm productDetail = new ProductDetailVm()
            {
                Product = product,
                RelatedProducts = await _context.Products.Include(x=>x.Category).Include(x=>x.ProductTags).ThenInclude(x=>x.Tag).Include(x=>x.ProductPhotos).Where(x => x.CategoryId == product.CategoryId).OrderByDescending(x => x.CreatedAt).Take(10).ToListAsync()
        };

            return View(productDetail);

        }

    }
}
