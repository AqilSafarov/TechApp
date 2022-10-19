using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechShop.Models;

namespace TechShop.ViewComponents
{
    public class VcSubscribe: ViewComponent
    {
        private readonly AppDbContext _context;

        public VcSubscribe(AppDbContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke()
        {
            Subscribe subscribe = new Subscribe();
            return View(subscribe);
        }
    }
}
