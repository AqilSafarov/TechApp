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
    public class TagController : Controller
    {
        private readonly AppDbContext _context;

        public TagController(AppDbContext context)
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

            TagVm tag = new TagVm
            {
                Tags = await _context.Tags.ToListAsync()
            };

            return View(tag);
        }
        public async Task<IActionResult> Create()
        {
            return View();

        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(Tag tag)
        {
            if (!ModelState.IsValid)
            return View();

            if (await _context.Tags.AnyAsync(x=>x.Name.ToLower()== tag.Name.Trim().ToLower()))
            {
                ModelState.AddModelError("Name","Bele bir tag movcutdur");
            }

            tag.CreatedAt = DateTime.UtcNow;
            tag.ModifideAt = DateTime.UtcNow;

            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();


            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {

            Tag tag = await _context.Tags.FirstOrDefaultAsync(x=>x.Id==id);

            if (tag==null)
            {
                return NotFound();
                     
            }

            return View(tag);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]

        public async Task<IActionResult> Edit(Tag tag)
        {

            Tag exsiTag = await _context.Tags.FirstOrDefaultAsync(x => x.Id == tag.Id);

            if (exsiTag == null)
            {
                return NotFound();

            }
            exsiTag.Name = tag.Name;
            exsiTag.ModifideAt = DateTime.UtcNow;

          await  _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int id)
        {
            Tag tag = await _context.Tags.FirstOrDefaultAsync(x => x.Id == id);

            if (tag == null)
            {
                return NotFound();
            }
            return View(tag);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]

        public async Task<IActionResult> Delete(Tag tag)
        {
            Tag exsitTag = await _context.Tags.FirstOrDefaultAsync(x => x.Id == tag.Id);

            if (exsitTag == null)
            {
                return NotFound();
            }

            _context.Tags.Remove(exsitTag);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");

        }
    }
}
