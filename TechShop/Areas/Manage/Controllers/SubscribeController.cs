using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechShop.Models;

namespace TechShop.Areas.Manage.Controllers
{
    [Area("manage")]
    public class SubscribeController : Controller
    {
        private readonly AppDbContext _context;

        public SubscribeController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page=1)
        {
            double totalCount = _context.Subscribes.Count();

            int pageCount = (int)Math.Ceiling(totalCount / 6);
                if (page<1)
            {
                page = 1;
            }
            else if(page>pageCount)
            {
                page = pageCount;
            }
            ViewBag.PageCount = pageCount;
            ViewBag.SelectedPage = page;
            List<Subscribe> subscribes = _context.Subscribes.Skip((page - 1) * 6).Take(6).ToList();
            return View(subscribes);
        }
        public IActionResult Delete(int id)
        {
            Subscribe subscribe = _context.Subscribes.FirstOrDefault(x=>x.Id==id);
            #region CheckkSubscribe
            if(subscribe==null)
                return NotFound();
            #endregion

            _context.Subscribes.Remove(subscribe);
            _context.SaveChanges();
            return Ok();


        }
    }
}
