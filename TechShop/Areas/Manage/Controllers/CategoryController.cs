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

    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            double totalCount = await _context.Categories.CountAsync();
            int pageCount = (int)Math.Ceiling(totalCount / 2);

            if (page < 1) page = 1;
            else if (page > pageCount) page = pageCount;

            ViewBag.PageCount = pageCount;
            ViewBag.SelectedPage = page;
            CategoryVm category = new CategoryVm
            {
                Categories = await _context.Categories.Where(x=>!x.IsDeleted).ToListAsync()
            };
            return View(category);
        }
        public async Task<IActionResult> Create()
        {
            Category lastCategory = await _context.Categories.Where(x => !x.IsDeleted).OrderByDescending(x => x.Order).FirstOrDefaultAsync();

            if (lastCategory != null)
            {
                ViewBag.BiggestOrder = lastCategory.Order;
            }
            else
            {
                ViewBag.BiggestOrder = 1;
            }

            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(CategoryCreateVm createVm)
        {
            if (!ModelState.IsValid)
                return View();

            if (await _context.Categories.AnyAsync(x=>!x.IsDeleted && x.Name.ToLower()==createVm.Name.Trim().ToLower()))
            {
                ModelState.AddModelError("Name","Bele bir Category artiq movcutdur");
                return View();

            }

            Category category = new Category
            {
                Order = createVm.Order,
                Name = createVm.Name.Trim(),
                CreatedAt = DateTime.UtcNow,
                ModifideAt= DateTime.UtcNow
            };

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();   


            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit(int id)
        {
            Category category = await _context.Categories.FirstOrDefaultAsync(x=> !x.IsDeleted && x.Id==id);

            if (category==null)
            {
                return NotFound();
            }

            CategoryCreateVm createVm = new CategoryCreateVm
            {
                Name = category.Name,
                Order = category.Order
            };
            ViewBag.Id = id;

            return View(createVm);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]

        public async Task<IActionResult> Edit(int id,CategoryCreateVm categoryVm)
        {

            Category category = await _context.Categories.FirstOrDefaultAsync(x=>!x.IsDeleted && x.Id==id);

            if (category==null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
                return View();

            if (await _context.Categories.AnyAsync(x=>x.Id!=id && x.Name.ToLower()== categoryVm.Name.Trim().ToLower()))
            {
                ModelState.AddModelError("Name","Bele bir category movcutdur");
            }

            category.Order = categoryVm.Order;
            category.Name = categoryVm.Name;
            category.CreatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();


            return RedirectToAction("index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            Category category = await _context.Categories.FirstOrDefaultAsync(x =>!x.IsDeleted && x.Id == id);

            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]

        public async Task<IActionResult> Delete(Category  categoryModel)
        {
            Category category = await _context.Categories.FirstOrDefaultAsync(x=>x.Id== categoryModel.Id);

            if (category == null)
            {
                return NotFound();
            }

            category.IsDeleted = true;
            await _context.SaveChangesAsync();

            return RedirectToAction("index");

        }
    }
}
