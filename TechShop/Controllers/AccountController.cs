using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TechShop.Models;
using TechShop.View_Models;

namespace TechShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(MemberLoginVm loginVm)
        {

            if (!ModelState.IsValid)
                return View();
            AppUser member = await _userManager.FindByEmailAsync(loginVm.Email);

            if (member==null)
            {
                ModelState.AddModelError("","Email ve ya sifre sehvdir");
                return View();

            }

            if (!await _userManager.CheckPasswordAsync(member,loginVm.Password))
            {
                ModelState.AddModelError("", "Email ve ya sifre sehvdir");
                return View();

            }

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[]
            {
               new Claim(ClaimTypes.Name,member.UserName),
               new Claim(ClaimTypes.Email,member.Email),
               new Claim(ClaimTypes.Role,"Member")

            },"Member_Auth");
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync("Member_Auth",claimsPrincipal);


            return RedirectToAction("Index","Home");
        }
        public IActionResult Register()
        {
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(MemberRegisterVm registerVm)
        {

            if (!ModelState.IsValid)
                return View();

            AppUser member = await _userManager.FindByEmailAsync(registerVm.Email);

            if (member != null)
            {
                ModelState.AddModelError("","Bele bir Email adresi artiq mocutdur");
                return View();
            }
            else
            {
                member = await _userManager.FindByNameAsync(registerVm.UserName);
                if (member != null)
                {
                    ModelState.AddModelError("", "Bele bir Istifadeci adresi artiq mocutdur");
                    return View();

                }
            }

            member = new AppUser
            {
                UserName = registerVm.UserName,
                Email = registerVm.Email,
                Fullname = registerVm.Fullname

            };

            var result = await _userManager.CreateAsync(member, registerVm.Password);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("",item.Description);
                }
                return View();
            }

            await _userManager.AddToRoleAsync(member,"Member");

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[]
             {
               new Claim(ClaimTypes.Name,member.UserName),
               new Claim(ClaimTypes.Email,member.Email),
               new Claim(ClaimTypes.Role,"Member")

            }, "Member_Auth");
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync("Member_Auth", claimsPrincipal);

            return RedirectToAction("Index","Home");

        }

        [Authorize(Roles= "Member",AuthenticationSchemes = "Member_Auth")]
        public async Task<IActionResult> Profile()
        {
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Member_Auth");

            return RedirectToAction("Index","Home");
        }
    }
}
