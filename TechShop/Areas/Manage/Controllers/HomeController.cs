using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechShop.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = "Admin_Auth")]

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
