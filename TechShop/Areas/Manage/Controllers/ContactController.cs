using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechShop.Models;

namespace TechShop.Areas.Manage.Controllers
{
    [Area("manage")]
    public class ContactController : Controller
    {
        private readonly AppDbContext _context;

        public ContactController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page=1)
        {
            double totalCount = _context.MessageContacts.Count();
            int pageCount = (int)Math.Ceiling(totalCount / 6);

            if (page < 1)
            {
                page = 1;
            }
            else if (page > pageCount)
            {
                page = pageCount;
            }

            ViewBag.PageCount = pageCount;
            ViewBag.SelectedPage = page;

            List<MessageContact> messageContacts = _context.MessageContacts.Skip((page - 1) * 6).Take(6).ToList();
            return View(messageContacts);
        }
       
        public async Task<IActionResult> Delete(MessageContact contact)
        {
            MessageContact existMessage = await _context.MessageContacts.FirstOrDefaultAsync(x => x.Id == contact.Id);

            if (existMessage == null)
            {
                return NotFound();
            }

            _context.MessageContacts.Remove(existMessage);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");

        }
    }
}
