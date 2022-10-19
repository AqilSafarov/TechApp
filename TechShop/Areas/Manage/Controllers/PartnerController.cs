using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechShop.Helpers;
using TechShop.Models;

namespace TechShop.Areas.Manage.Controllers
{
    [Area("manage")]
    public class PartnerController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public PartnerController(AppDbContext context,IWebHostEnvironment env )
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult>Index(int page=1)
        {
            double totalCount =await _context.Partners.CountAsync();
            int pageCount = (int)Math.Ceiling(totalCount/6);

            if (page>1)
            {
                page = 1;
            }
            else if (page>pageCount)
            {
                page = pageCount;
            }
            ViewBag.PageCount = pageCount;
            ViewBag.SelectedPage = page;

            List<Partner> partner = await _context.Partners.ToListAsync();

            return View(partner);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(Partner partner)
        {
           

            if (partner.ImageFile!=null)
            {
                #region ChehckFileRaneg
                if (partner.ImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("ImageFile", "2mb artiq ola bilmez");
                    return View(partner);
                }
                #endregion

                #region ChechkFileContentType
                if (partner.ImageFile.ContentType != "image/png" && partner.ImageFile.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("ImageFile", "File png,jpeg olamlidir");


                    return View(partner);
                }

                #endregion


                string filename = FileManager.Save(_env.WebRootPath, "uploads/partners", partner.ImageFile);

                partner.Photo = filename;
            }

            partner.CreatedAt = DateTime.UtcNow;
            partner.ModifideAt = DateTime.UtcNow;

            await _context.AddAsync(partner);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit(int id)
        {
            Partner partner = await _context.Partners.FirstOrDefaultAsync(x => x.Id == id);

            #region CheckPartner
            if (partner == null)
            {
                return NotFound();

            }

            #endregion

            return View(partner);

        
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Edit(Partner partner)
        {
            Partner existPartner = await _context.Partners.FirstOrDefaultAsync(x=>x.Id== partner.Id);
            #region CheheckExistProduct
            if (existPartner == null)
            {
                return NotFound();
            }
            #endregion

            if (partner.ImageFile != null)
            {
                #region CheckFileLength
                if (partner.ImageFile.Length > 2 * (1024 * 1024))
                {
                    ModelState.AddModelError("File", "Sekil olcusu 2MB-dan boyuk ola bilmez ,qaqa!!!");
                    return View();
                }
                #endregion

                #region CheckFileContentType
                if (partner.ImageFile.ContentType != "image/png" && partner.ImageFile.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("File", "Seklin novunu duz sec!!!");
                    return View();
                }
                #endregion

                string filename = FileManager.Save(_env.WebRootPath, "uploads/partners", partner.ImageFile);

                if (!string.IsNullOrWhiteSpace(existPartner.Photo))
                {
                    FileManager.Delete(_env.WebRootPath, "uploads/partners", existPartner.Photo);
                }

                existPartner.Photo = filename;
            }
            else if (!string.IsNullOrWhiteSpace(existPartner.Photo))
            {
                FileManager.Delete(_env.WebRootPath, "uploads/partners", existPartner.Photo);


                existPartner.Photo = null;
            }


            await _context.SaveChangesAsync();

            return RedirectToAction("index");

        }


        public async Task<IActionResult> Delete(int id)
        {
            Partner partner = await _context.Partners.FirstOrDefaultAsync(x => x.Id == id);


            #region CheckSliderNotFound
            if (partner == null)
            {
                return NotFound();
            }
            #endregion

            if (!string.IsNullOrWhiteSpace(partner.Photo))
            {
                FileManager.Delete(_env.WebRootPath, "uploads/partners", partner.Photo);
            }


            _context.Partners.Remove(partner);
            _context.SaveChanges();

            return Ok();
        }
    }
}
