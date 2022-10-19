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
    public class ContactController : Controller
    {
        private readonly AppDbContext _context;

        public ContactController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
          
            ContactVm contactVm = new ContactVm
            {
                Setting = _context.Settings.FirstOrDefault(),
               


            };
            return View(contactVm);
        }
        public IActionResult Message(ContactVm model)
        {
            if (!ModelState.IsValid)
            {
                return View();

            }
            model.MessageContact.CreatedAt = DateTime.UtcNow;
            model.MessageContact.ModifideAt = DateTime.UtcNow;

            _context.Add(model.MessageContact);
            _context.SaveChanges();

            return RedirectToAction("Index", "Contact");

        }

    }
}
