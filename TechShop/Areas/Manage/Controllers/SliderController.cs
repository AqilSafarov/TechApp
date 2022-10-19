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
    public class SliderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SliderController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index(int page=1)
        {
            double totalCount = await _context.Sliders.CountAsync();
            int pageCount = (int)Math.Ceiling(totalCount/6);

            if (page<1)
            {
                page = 1;
            }
            else if(page>pageCount)
            {
                page = pageCount;
            }

            ViewBag.PageCount = pageCount;
            ViewBag.SelectedPage = page;
            List<Slider> sliders = await _context.Sliders.Skip((page - 1) * 6).Take(6).ToListAsync();
            return View(sliders);
            }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Slider slider)
        {
            #region ModelState
            if (!ModelState.IsValid)
            {
                return View();
            }
            #endregion

            if (slider.ImageFile != null)
            {

                if (slider.ImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("ImageFile", "2 mq artiq ola bilmez");


                    return View(slider);
                }
                if (!(slider.ImageFile.ContentType == "image/png" || slider.ImageFile.ContentType == "image/jpeg" || slider.ImageFile.ContentType == "image/gif"))
                {
                    ModelState.AddModelError("ImageFile", "File png,jpeg olamlidir");


                    return View(slider);
                }

                string filename = FileManager.Save(_env.WebRootPath, "uploads/sliders", slider.ImageFile);

                slider.Photo = filename;

            }

            slider.CreatedAt = DateTime.UtcNow;
            slider.ModifideAt = DateTime.UtcNow;

            await _context.AddAsync(slider);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit(int id)
        {
            Slider slider = await _context.Sliders.FirstOrDefaultAsync(x=>x.Id==id);

            #region SliderChehck
            if (slider == null)
            {
                return NotFound();
            }

            #endregion
            return View(slider);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(Slider slider)
        {
            Slider existSlider = await _context.Sliders.FirstOrDefaultAsync(x => x.Id == slider.Id);
            #region CheheckExistSlider
            if (existSlider == null)
            {
                return NotFound();
            }
            #endregion


            existSlider.Title = slider.Title;
            existSlider.Text = slider.Text;
            existSlider.Price = slider.Price;
            existSlider.RedirectUrl = slider.RedirectUrl;


            if (slider.ImageFile != null)
            {
                #region CheckFileLength
                if (slider.ImageFile.Length > 2 * (1024 * 1024))
                {
                    ModelState.AddModelError("File", "Sekil olcusu 2MB-dan boyuk ola bilmez ,qaqa!!!");
                    return View();
                }
                #endregion

                #region CheckFileContentType
                if (slider.ImageFile.ContentType != "image/png" && slider.ImageFile.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("File", "Seklin novunu duz sec!!!");
                    return View();
                }
                #endregion

                string filename = FileManager.Save(_env.WebRootPath, "uploads/sliders", slider.ImageFile);

                if (!string.IsNullOrWhiteSpace(existSlider.Photo))
                {
                    FileManager.Delete(_env.WebRootPath, "uploads/sliders", existSlider.Photo);
                }

                existSlider.Photo = filename;
            }
            #region AnotherCheck
            //else if (slider.Photo==null)
            //{

            //    if (existSlider.Photo==null)
            //    {
            //        slider.Photo = null;

            //    }
            //    else
            //    {
            //        FileManager.Delete(_env.WebRootPath, "uploads/sliders", existSlider.Photo);

            //        existSlider.Photo = null;
            //    }


            //}
            #endregion

            else if (!string.IsNullOrWhiteSpace(existSlider.Photo))
            {
                FileManager.Delete(_env.WebRootPath, "uploads/sliders", existSlider.Photo);


                existSlider.Photo = null;
            }

            //_context.Sliders.Update(slider);

            await _context.SaveChangesAsync();

            return RedirectToAction("index");


        }

        
        public async Task<IActionResult> Delete(int id)
        {
            Slider slider = _context.Sliders.FirstOrDefault(x => x.Id == id);

            #region CheckSliderNotFound
            if (slider == null)
            {
                return NotFound();
            }
            #endregion

            if (!string.IsNullOrWhiteSpace(slider.Photo))
            {
                FileManager.Delete(_env.WebRootPath, "uploads/sliders", slider.Photo);
            }


            _context.Sliders.Remove(slider);
            _context.SaveChanges();

            return Ok();
        }
    }
}
