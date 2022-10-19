using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechShop.Models;
using TechShop.View_Models;

namespace TechShop.ViewComponents
{
    public class VcNavbar: ViewComponent
    {
        private readonly AppDbContext _context;

        public VcNavbar(AppDbContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke()
        {
            List<Category> categories = _context.Categories.Where(x => (x.Name == "Phone")||(x.Name== "Smartwatch")).ToList();

            VmNavSetCat vm = new VmNavSetCat()
            {
                CategoryPhone = _context.Categories.Where(x=>x.Name == "Phone").FirstOrDefault(),
                CategoryWatch= _context.Categories.Where(x => x.Name == "Smartwatch").FirstOrDefault(),
                Setting =_context.Settings.FirstOrDefault()

        };

            return View(vm);
        }
    }
}
