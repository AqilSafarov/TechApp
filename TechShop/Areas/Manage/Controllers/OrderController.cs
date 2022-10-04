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

    public class OrderController : Controller
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
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
            List<Order> orders = await _context.Orders.Include(x => x.Product).Include(x => x.AppUser).ToListAsync();
            return View(orders);
        }

        public async Task<IActionResult> Edit(int id)
        {
            Order order = await _context.Orders.Include(x => x.Product).Include(x => x.AppUser).FirstOrDefaultAsync(x=>x.Id==id);

            if (order == null)
                return NotFound();

            return View(order);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Accept(int id)
        {
            Order order = await _context.Orders.FirstOrDefaultAsync(x=>x.Id==id);
            if (order == null)
                return NotFound();

            order.Status = Helpers.OrderStatus.Accepted;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]

        public async Task<IActionResult> Reject(int id)
        {
            Order order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == id);
            if (order == null)
                return NotFound();

            order.Status = Helpers.OrderStatus.Rejected;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
