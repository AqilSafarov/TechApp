using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TechShop.Models;
using TechShop.View_Models;

namespace TechShop.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public OrderController(AppDbContext context,UserManager<AppUser>userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [Authorize(Roles = "Member", AuthenticationSchemes = "Member_Auth")]

        public async Task<IActionResult> Index()
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            //string userId = User.FindFirst("NameIdentifier").Value;
            List<Order> orders = await _context.Orders.Include(x=>x.Product).Where(x => x.AppUserId == userId).OrderByDescending(x => x.CreatedAt).ToListAsync();

            return View(orders);
        }
        [HttpPost]
        [Authorize(Roles = "Member", AuthenticationSchemes = "Member_Auth")]
        public async Task<IActionResult> Create(Order order)
        {

            Product product = _context.Products.FirstOrDefault(x => x.Id == order.ProductId);
            if (product == null)
            {
                return NotFound();
            }
            //AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            List<BasketCardItemModel> basketItem = new List<BasketCardItemModel>();

            if (Request.Cookies["basket"] == null)
            {
                //basketCardVm.TotalPrice = book.Price;
                BasketCardItemModel basketCardItemModel = new BasketCardItemModel
                {
                    Id = order.ProductId,
                    Count = 1
                };
                basketItem.Add(basketCardItemModel);
            }
            else
            {
                basketItem = JsonConvert.DeserializeObject<List<BasketCardItemModel>>(Request.Cookies["basket"]);

                //basketCardVm.TotalPrice += book.Price;

                if (basketItem.Any(b => b.Id == order.ProductId))
                {
                    BasketCardItemModel basketBasketItem = basketItem.FirstOrDefault(x => x.Id == order.ProductId);
                    basketBasketItem.Count += 1;
                }
                else
                {
                    BasketCardItemModel basketCardItemModel = new BasketCardItemModel
                    {
                        Id = order.ProductId,

                        Count = 1
                    };
                    basketItem.Add(basketCardItemModel);
                }
            }


            Response.Cookies.Append("basket", JsonConvert.SerializeObject(basketItem), new CookieOptions { MaxAge = TimeSpan.FromDays(1) });

            //Product product = await _context.Products.FirstOrDefaultAsync(x=>x.Id==order.ProductId);
                
            //if (product == null)
            //    return NotFound();

            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            order.AppUserId = user.Id;
            order.Price = product.Price;
            order.DiscountPrice = product.DiscountPrice;
            order.Status = Helpers.OrderStatus.Pending;
            order.CreatedAt = DateTime.UtcNow;
            order.ModifideAt = DateTime.UtcNow;

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();


            return RedirectToAction("Index");
        }
    }
}
