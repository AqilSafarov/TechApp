using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TechShop.Areas.Manage.ViewModels;
using TechShop.Models;

namespace TechShop.Areas.Manage.Controllers
{
    [Area("manage")]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager,RoleManager<IdentityRole> roleManager,SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Login(AdminLoginVm loginVm)
        {

            if (!ModelState.IsValid)
            {
                return View();

            }
            AppUser admin = await _userManager.FindByNameAsync(loginVm.UserName);

            if (admin==null)
            {
                ModelState.AddModelError("","Istifadeci adi ve ya parol sehvdir");
                return View();
                 
            }

            //Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(admin, loginVm.Password, false, false);

            if (await _userManager.CheckPasswordAsync(admin,loginVm.Password))
            {
                ModelState.AddModelError("", "Istifadeci adi ve ya parol sehvdir");
                return View();
            }


            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[]
           {
               new Claim(ClaimTypes.Name,admin.UserName),
               new Claim(ClaimTypes.Role,"Admin")

            }, "Admin_Auth");
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync("Admin_Auth", claimsPrincipal);

            return RedirectToAction("Index","Home");
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Admin_Auth");

            return RedirectToAction("Index", "Home");
        }

        #region CreateRole

        //public async Task Create()
        //{
        //    AppUser user = new AppUser
        //    {
        //        UserName = "SuperAdmin",
        //        Fullname = "Super Admin",
        //    };

        //    await _userManager.CreateAsync(user, "Admin123");
        //    await _userManager.AddToRoleAsync(user, "Admin");
        //}
        //public async Task CreateRole()
        //{
        //    await _roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
        //    await _roleManager.CreateAsync(new IdentityRole { Name = "Member" });

        //}
        #endregion

    }
}
