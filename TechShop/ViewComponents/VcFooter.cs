using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechShop.Models;

namespace TechShop.ViewComponents
{
    public class VcFooter: ViewComponent
    {
        private readonly AppDbContext _context;

        public VcFooter(AppDbContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke()
        {
            Setting setting = _context.Settings.FirstOrDefault();

            return View(setting);
        }
    }
}
