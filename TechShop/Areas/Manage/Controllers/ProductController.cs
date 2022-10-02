using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechShop.Areas.Manage.ViewModels;
using TechShop.Helpers;
using TechShop.Models;

namespace TechShop.Areas.Manage.Controllers
{
    [Area("manage")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index(int page=1)
        {

            double totalCount = await _context.Categories.CountAsync();
            int pageCount = (int)Math.Ceiling(totalCount / 2);

            if (page < 1) page = 1;
            else if (page > pageCount) page = pageCount;

            ViewBag.PageCount = pageCount;
            ViewBag.SelectedPage = page;
            ProductVm productVm = new ProductVm
            {
                Products = await _context.Products.Include(x=>x.Category).ToListAsync()
        };
            return View(productVm);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.Where(x => !x.IsDeleted).ToListAsync();
            ViewBag.Tags = await _context.Tags.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            #region ModelState
            if (!ModelState.IsValid)
            {
                return View();
            }
            #endregion
            #region CheheckSlug
            if (await _context.Products.AnyAsync(x => x.Slug.ToLower() == product.Slug.Trim().ToLower()))
            {
                ViewBag.Categories = await _context.Categories.Where(x => !x.IsDeleted).ToListAsync();
                ViewBag.Tags = await _context.Tags.ToListAsync();
                ModelState.AddModelError("Slug", "Bele bir mehsul artiq movcutdur");
                return View();

            }

            #endregion

            #region ChehckCategory

            if (!await _context.Categories.AnyAsync(x=>x.Id==product.CategoryId && !x.IsDeleted))  //slinememis ve gonderilmis id li data yoxdursa
            {
                ModelState.AddModelError("CategoryId", "Bele bir Category yoxdur");
                return View();
            }

            product.ProductTags = new List<ProductTag>();

            foreach (var tagId in product.TagIds)
            {
                if (!await _context.Tags.AnyAsync(x=>x.Id==tagId))
                {
                    ModelState.AddModelError("TagIds","Bele bir tag yoxdur");
                    return View();
                }

                ProductTag productTag = new ProductTag
                {
                    Product = product,
                    TagId = tagId
                };

                product.ProductTags.Add(productTag);

            }

            product.ProductPhotos = new List<ProductPhoto>();
            int photoOrder = 1;
            foreach (var file in product.Files)
            {

                #region ChechkFileRange
                if (file.Length > 2 * (1024 * 1024))
                {
                    ModelState.AddModelError("File", "2 mq artiq ola bilmez");
                    return View();

                }
                #endregion

                #region ChehckFileContentTye
                if (file.ContentType != "image/png" && file.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("File", "File png,jpeg olamlidir");
                    return View();

                }
                #endregion

                string filename = FileManager.Save(_env.WebRootPath, "uploads/products", file);

                ProductPhoto productPhoto = new ProductPhoto()
                {
                    Name = filename,
                    Order = photoOrder,
                    Product = product,

                };
            photoOrder++;

                product.ProductPhotos.Add(productPhoto);
            }


            product.CreatedAt = DateTime.UtcNow;
            product.ModifideAt = DateTime.UtcNow;
            product.DiscountPrice = product.DiscountPercent <= 0 ? product.Price : (product.Price * (100 - product.DiscountPercent) / 100);


            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            #endregion


            return RedirectToAction("Index");

        }
        public async Task<IActionResult> Edit(int id)
        {
            Product product = await _context.Products.Include(x => x.ProductTags).Include(x => x.ProductPhotos).FirstOrDefaultAsync(x=>x.Id==id);

            #region ProductChehck
            if (product == null)
            {
                return NotFound();
            }

            #endregion

            ViewBag.Categories = await _context.Categories.Where(x => !x.IsDeleted).ToListAsync();
            ViewBag.Tags = await _context.Tags.ToListAsync();

            return View(product);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product)
        {
            Product existProduct = await _context.Products.Include(x=>x.ProductPhotos).Include(x=>x.ProductTags).FirstOrDefaultAsync(x=>x.Id==product.Id);

            #region CheheckExistProduct
            if (existProduct==null)
            {
                return NotFound();
            }
            #endregion

            #region CheheckCaetgory
            if (!await _context.Categories.AnyAsync(x => x.Id == product.CategoryId))
            {
                return NotFound();
            }
            #endregion

            #region CheheckSlug
            if (await _context.Products.AnyAsync(x=>x.Slug.ToLower()==product.Slug.ToLower() && x.Id!=product.Id))
            {
                return NotFound();  

            }
            #endregion

            List<ProductTag> removeableTags = new List<ProductTag>();
            removeableTags.AddRange(existProduct.ProductTags) ;

            foreach (var tagId in product.TagIds)
            {
                ProductTag tag = existProduct.ProductTags.FirstOrDefault(x=>x.TagId== tagId);

                if (tag!=null)
                {
                    removeableTags.Remove(tag);
                }
                else
                {
                    if (!await _context.Tags.AnyAsync(x => x.Id == tagId))
                        return NotFound();

                    tag = new ProductTag
                    {
                        TagId = tagId,
                        ProductId=product.Id
                    };

                    existProduct.ProductTags.Add(tag);
                }
            }

            existProduct.ProductTags = existProduct.ProductTags.Except(removeableTags).ToList();

            List<ProductPhoto> removeablePhotos = new List<ProductPhoto>();

            foreach (var item in existProduct.ProductPhotos)
            {
                if (product.FileIds.Any(x => x == item.Id))  //Sekillerin hamisini silende prablem yaranir
                    continue;

                FileManager.Delete(_env.WebRootPath, "uploads/products", item.Name);
                removeablePhotos.Add(item);
            }

            existProduct.ProductPhotos = existProduct.ProductPhotos.Except(removeablePhotos).ToList();

            var lastPhoto = existProduct.ProductPhotos.OrderByDescending(x => x.Order).FirstOrDefault();
            int photoOrder = lastPhoto != null ? lastPhoto.Order + 1:1 ;

            

            foreach (var file in product.Files)
            {

                #region ChechkFileRange
                if (file.Length > 2 * (1024 * 1024))
                {
                    ModelState.AddModelError("File", "2 mq artiq ola bilmez");
                    return View();

                }
                #endregion

                #region ChehckFileContentTye
                if (file.ContentType != "image/png" && file.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("File", "File png,jpeg olamlidir");
                    return View();

                }
                #endregion

                string filename = FileManager.Save(_env.WebRootPath, "uploads/products", file);

                ProductPhoto productPhoto = new ProductPhoto()
                {
                    Name = filename,
                    Order = photoOrder,
                    Product = product,

                };
                photoOrder++;

                existProduct.ProductPhotos.Add(productPhoto);
            }



            if (existProduct.Price != product.Price || existProduct.DiscountPercent != product.DiscountPercent)
            {
                product.DiscountPercent = product.DiscountPercent <= 0 ? product.Price : (product.Price * (100 - product.DiscountPercent) / 100);

            }

            existProduct.CategoryId = product.CategoryId;
            existProduct.Price = product.Price;
            existProduct.ProducingPrice = product.ProducingPrice;
            existProduct.DiscountPrice = product.DiscountPrice;
            existProduct.Desc = product.Desc;
            existProduct.InfoText = product.InfoText;
            existProduct.Name = product.Name;
            existProduct.Slug = product.Slug;
            existProduct.IsAvailable = product.IsAvailable;
            existProduct.ModifideAt =DateTime.UtcNow;


            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Review(int productId)
        {
            List<ProductReview> productReviews = await _context.ProductReviews.Where(x => x.ProductId == productId).ToListAsync();

            return View(productReviews);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReview(int id)
        {
            ProductReview review = await _context.ProductReviews.FirstOrDefaultAsync(x=>x.Id== id);
            if (review==null)
            {
                return NotFound();
            }

            _context.ProductReviews.Remove(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
