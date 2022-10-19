using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechShop.Models;

namespace TechShop.ViewComponents
{
    public class VcPartner: ViewComponent
    {
        private readonly AppDbContext _context;

        public VcPartner(AppDbContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke()
        {
           List< Partner> partner = _context.Partners.ToList();
            return View(partner);

        }
    }
}
