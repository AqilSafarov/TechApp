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
    public class SettingController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SettingController(AppDbContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            List<Setting> settings = _context.Settings.ToList();
            return View(settings);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Setting setting)
        {
            #region ModelSatate
            if (!ModelState.IsValid)
            {
                return View(setting);

            }
            #endregion

            if (setting.ImageFileNav!=null)
            {
                #region FileRange
                if (setting.ImageFileNav.Length > 2097152)
                {
                    ModelState.AddModelError("ImageFileFooter", "2 mq artiq ola bilmez");

                }
                #endregion
                #region ContentTypeCheck
                if (!(setting.ImageFileNav.ContentType == "image/png" || setting.ImageFileNav.ContentType == "image/jpeg"))
                {
                    ModelState.AddModelError("ImageFile", "File png,jpeg olamlidir");


                    return View(setting);
                }
                #endregion

               

                string filenameNav = FileManager.Save(_env.WebRootPath, "uploads/settings", setting.ImageFileNav);

                setting.LogoNav = filenameNav;
            }

            setting.CreatedAt = DateTime.UtcNow;
            setting.ModifideAt = DateTime.UtcNow;

            await _context.AddAsync(setting);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");

        }
        public IActionResult Edit(int id)
        {
            Setting setting = _context.Settings.FirstOrDefault(x=>x.Id==id);

            if (setting==null)
            {
                return NotFound();

            }
            return View(setting);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Setting setting)
        {
            Setting existSettig = await _context.Settings.FirstOrDefaultAsync(x=>x.Id==setting.Id);

            #region CheheckExistSetting
            if (existSettig == null)
            {
                return NotFound();
            }
            #endregion

            existSettig.About = setting.About;
            existSettig.Address = setting.Address;
            existSettig.ContactNumPhone = setting.ContactNumPhone;
            existSettig.EmailContact = setting.EmailContact;
            existSettig.EmailNav = setting.EmailNav;
            existSettig.PhoneNumNav = setting.PhoneNumNav;
            existSettig.WorkClose = setting.WorkClose;
            existSettig.WorkStart = setting.WorkStart;

            if (setting.ImageFileNav != null)
            {
                #region FileRange
                if (setting.ImageFileNav.Length > 2097152)
                {
                    ModelState.AddModelError("ImageFileFooter", "2 mq artiq ola bilmez");

                }
                #endregion
                #region ContentTypeCheck
                if (!(setting.ImageFileNav.ContentType == "image/png" || setting.ImageFileNav.ContentType == "image/jpeg"))
                {
                    ModelState.AddModelError("ImageFile", "File png,jpeg olamlidir");


                    return View(setting);
                }
                #endregion



                string filenameNav = FileManager.Save(_env.WebRootPath, "uploads/settings", setting.ImageFileNav);

                if (!string.IsNullOrWhiteSpace(existSettig.LogoNav))
                {
                    FileManager.Delete(_env.WebRootPath, "uploads/settings", existSettig.LogoNav);
                }
                existSettig.LogoNav = filenameNav;

            }
            else if (!string.IsNullOrWhiteSpace(existSettig.LogoNav))
            {
                FileManager.Delete(_env.WebRootPath, "uploads/sliders", existSettig.LogoNav);


                existSettig.LogoNav = null;
            }
            await _context.SaveChangesAsync();

            return RedirectToAction("index");
        }
        public async Task<IActionResult> Delete(int id)
        {
            Setting setting = await _context.Settings.FirstOrDefaultAsync(x=>x.Id==id);

            #region CheckSettingNotFound
            if (setting == null)
            {
                return NotFound();
            }
            #endregion

            if (!string.IsNullOrWhiteSpace(setting.LogoNav))
            {
                FileManager.Delete(_env.WebRootPath, "uploads/settings", setting.LogoNav);

            }
            _context.Settings.Remove(setting);
            _context.SaveChanges();

            return Ok();
        }
    }
}
