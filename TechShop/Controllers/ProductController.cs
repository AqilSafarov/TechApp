using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechShop.Models;
using TechShop.View_Models;

namespace TechShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ProductController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(string searchData,int page = 1, int? categoryId = null)
        {

      
            decimal pageCount = _context.Products.Where(x => (categoryId != null ? x.CategoryId == categoryId : true)).Count() / 8m;



            ViewBag.SelectedPage = page;
            ViewBag.PageCount = (int)Math.Ceiling(pageCount);
            ViewBag.CategoryId = categoryId;
         
            List<Product> product = await _context.Products.Where(x => (categoryId != null ? x.CategoryId == categoryId : true)).Where(p => (!string.IsNullOrEmpty(searchData) ? p.Name.Contains(searchData) : true) ||
                                          (!string.IsNullOrEmpty(searchData) ? p.Desc.Contains(searchData) : true)).Include(x => x.ProductPhotos)
             .Include(x => x.Category)
             .Include(x => x.ProductReviews)
             .Include(x => x.ProductTags)
             .ThenInclude(x => x.Tag).Skip((page - 1) * 8).Take(8).ToListAsync();
            ProductVm productVm = new ProductVm()
            {
                Products = product,
                Categories = await _context.Categories.ToListAsync(),
                Tags = await _context.Tags.ToListAsync()
            };
            return View(productVm);
        }
        public async Task<IActionResult> Detail(int id)
        {
            Product product = await _context.Products.Include(x=>x.ProductPhotos)
                .Include(x=>x.Category)
                .Include(x => x.ProductReviews)
                .Include(x=>x.ProductTags)
                .ThenInclude(x=>x.Tag).FirstOrDefaultAsync(x => x.Id == id);


            if (product == null)
                return NotFound();


            ProductDetailVm productDetail = new ProductDetailVm()
            {
                Product = product,
                RelatedProducts = await _context.Products.Include(x=>x.Category).Include(x=>x.ProductTags).ThenInclude(x=>x.Tag).Include(x=>x.ProductPhotos).Where(x => x.CategoryId == product.CategoryId).OrderByDescending(x => x.CreatedAt).Take(10).ToListAsync()
        };

            return View(productDetail);

        }
        public async Task<IActionResult> Fade(int id)
        {
            Product product = await _context.Products.Include(x => x.ProductPhotos)
              .Include(x => x.Category)
              .Include(x => x.ProductReviews)
              .Include(x => x.ProductTags)
              .ThenInclude(x => x.Tag).FirstOrDefaultAsync(x => x.Id == id);


            if (product == null)
                return NotFound();

            ProductDetailVm productDetail = new ProductDetailVm()
            {
                Product = product,
                //RelatedProducts = await _context.Products.Include(x => x.Category).Include(x => x.ProductTags).ThenInclude(x => x.Tag).Include(x => x.ProductPhotos).Where(x => x.CategoryId == product.CategoryId).OrderByDescending(x => x.CreatedAt).Take(10).ToListAsync()
            };

            return ViewComponent("VcFooter");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Review(ProductReview review)
        {
            Product product = await _context.Products.Include(x=>x.ProductReviews).FirstOrDefaultAsync(x => x.Id == review.ProductId);
            #region ChechkProduct
            if (product == null)
            {
                return NotFound();
            }
            #endregion

            ProductReview productReview = new ProductReview
            {
                CreatedAt = DateTime.UtcNow,
                Email = review.Email,
                Fullname = review.Fullname,
                Rate = review.Rate,
                ProductId=review.ProductId,
                Message = review.Message
            };

            product.ProductReviews.Add(productReview);
            product.Rate = product.ProductReviews.Sum(x => x.Rate) / product.ProductReviews.Count();

            await _context.ProductReviews.AddAsync(productReview);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AddBasket(int id)
        {

            Product product = _context.Products.FirstOrDefault(x => x.Id == id);
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
                    Id = id,
                    Count = 1
                };
                basketItem.Add(basketCardItemModel);
            }
            else
            {
                basketItem = JsonConvert.DeserializeObject<List<BasketCardItemModel>>(Request.Cookies["basket"]);

                //basketCardVm.TotalPrice += book.Price;

                if (basketItem.Any(b => b.Id == id))
                {
                    BasketCardItemModel basketBasketItem = basketItem.FirstOrDefault(x => x.Id == id);
                    basketBasketItem.Count += 1;
                }
                else
                {
                    BasketCardItemModel basketCardItemModel = new BasketCardItemModel
                    {
                        Id = id,
                        Count = 1
                    };
                    basketItem.Add(basketCardItemModel);
                }
            }


            Response.Cookies.Append("basket", JsonConvert.SerializeObject(basketItem), new CookieOptions { MaxAge = TimeSpan.FromDays(1) });

            return RedirectToAction("index");
        }
        public IActionResult DeleteBasket(int id)
        {
            List<BasketCardItemModel> basketItem = new List<BasketCardItemModel>();
            BasketCardItemModel basketCardItemModel = new BasketCardItemModel();

            basketItem = JsonConvert.DeserializeObject<List<BasketCardItemModel>>(Request.Cookies["basket"]);

            if (basketItem.Any(b => b.Id == id))
            {
                BasketCardItemModel basketBasketItem = basketItem.FirstOrDefault(x => x.Id == id);
                if (basketBasketItem.Count > 1)
                {
                    basketBasketItem.Count -= 1;

                }
                else
                {
                    basketItem.Remove(basketBasketItem);

                }
            }
            Response.Cookies.Append("basket", JsonConvert.SerializeObject(basketItem), new CookieOptions { MaxAge = TimeSpan.FromDays(1) });

            return RedirectToAction("index");
        }
        public IActionResult Basket()
        {
            var basket = Request.Cookies["basket"];
            List<BasketCardItemModel> basketItems = new List<BasketCardItemModel>();
            if (basket != null)
            {
                basketItems = JsonConvert.DeserializeObject<List<BasketCardItemModel>>(basket);

            }
            return Json(basketItems);
        }



    }
}
