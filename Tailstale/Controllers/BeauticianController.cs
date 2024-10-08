﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRUD_COREMVC;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Tailstale.Models;

namespace Tailstale.Controllers
{
    [IsLoginFilter]
    public class BeauticianController : Controller
    {
        private readonly TailstaleContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public BeauticianController(TailstaleContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: Beautician
        public async Task<IActionResult> Index()
        {
            //var tailstaleContext = _context.Beautician.Include(b => b.business);
            //return View(await tailstaleContext.ToListAsync());

           
            int? loginID = HttpContext.Session.GetInt32("loginID");
            int? loginType = HttpContext.Session.GetInt32("loginType");

           

            var Beauticians = await _context.Beauticians
            .Where(b => b.business_ID == loginID)
            .ToListAsync();

           
            return View(Beauticians);
        }

        



        // GET: Beautician/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var beautician = await _context.Beauticians
                .Include(b => b.business)
                .FirstOrDefaultAsync(m => m.id == id);
            if (beautician == null)
            {
                return NotFound();
            }

            return View(beautician);
        }

        // GET: Beautician/Create
        public  IActionResult Create()
        {
            int? loginID = HttpContext.Session.GetInt32("loginID");
            var Beauticians =  _context.businesses
           .Where(b => b.ID == loginID)
           .ToList();

            ViewData["business_ID"] = new SelectList(Beauticians, "ID", "name");
            //ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name");
            return View();
        }

        // POST: Beautician/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,name,gender,photo,phone,business_ID,Highest_license,Remark")] BeauticianViewModel beautician)
        {
            var businesses3 = _context.businesses
            .Where(b => b.type_ID == 2)
            .ToList();

            int? loginID = HttpContext.Session.GetInt32("loginID");
            var Beauticians = _context.businesses
           .Where(b => b.ID == loginID)
           .ToList();

            ViewData["business_ID"] = new SelectList(Beauticians, "ID", "name");
            if (ModelState.IsValid)
            {
                
                // 处理第一个上传的文件（Photo）
                if (HttpContext.Request.Form.Files.Count > 0)
                {
                    var photoFile = HttpContext.Request.Form.Files[0]; // 获取第一个上传的文件

                    if (photoFile != null && photoFile.Length > 0) // 检查文件是否有效
                    {
                        // 检查文件类型是否为图片
                        if (!IsImageFile(photoFile))
                        {
                            

                            ViewData["business_ID"] = new SelectList(Beauticians, "ID", "name");
                            ModelState.AddModelError("photo", "Only image files are allowed for Photo.");
                            //ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", beautician.business_ID);
                            return View(beautician);
                        }

                        // 生成唯一的文件名，避免重复
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(photoFile.FileName);

                        // 指定文件的完整路径，将文件保存到 wwwroot/images 文件夹下
                        string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "Salon_img", uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await photoFile.CopyToAsync(stream); // 将文件复制到指定路径
                        }

                        // 将文件名（包含扩展名）保存到 beautician 对象的 photo 属性
                        beautician.photo = uniqueFileName;
                    }
                }

                // 处理第二个上传的文件（Highest_license）
                if (HttpContext.Request.Form.Files.Count > 1)
                {
                    var licenseFile = HttpContext.Request.Form.Files[1]; // 获取第二个上传的文件

                    if (licenseFile != null && licenseFile.Length > 0) // 检查文件是否有效
                    {
                        // 检查文件类型是否为图片
                        if (!IsImageFile(licenseFile))
                        {
                           

                            ViewData["business_ID"] = new SelectList(Beauticians, "ID", "name");
                            ModelState.AddModelError("Highest_license", "Only image files are allowed for Highest License.");
                            //ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", beautician.business_ID);
                            return View(beautician);
                        }

                        // 生成唯一的文件名，避免重复
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(licenseFile.FileName);

                        // 指定文件的完整路径，将文件保存到 wwwroot/images 文件夹下
                        string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "Salon_img", uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await licenseFile.CopyToAsync(stream); // 将文件复制到指定路径
                        }

                        // 将文件名（包含扩展名）保存到 beautician 对象的 Highest_license 属性
                        beautician.Highest_license = uniqueFileName;
                    }
                }
                Beautician a = new Beautician()
                {
                    
                    name = beautician.name,
                    gender = beautician.gender,
                    photo = beautician.photo,
                    phone = beautician.phone,
                    business_ID = beautician.business_ID,
                    Highest_license = beautician.Highest_license,
                    Remark = beautician.Remark
                };
                // 将 beautician 对象添加到数据库上下文并保存更改
                //_context.Add(beautician);
                _context.Add(a); //只有寫入資料庫才需要把viewmodel轉model,下面的return其實不會管型態,沒差
                await _context.SaveChangesAsync();
                

                ViewData["business_ID"] = new SelectList(Beauticians, "ID", "name");
                //ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", beautician.business_ID);
                // 成功保存后重定向到 Index 页面
                return RedirectToAction(nameof(Index));
            }

            

            ViewData["business_ID"] = new SelectList(Beauticians, "ID", "name");
            //ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", beautician.business_ID);
            return View(beautician);



           
        }


        private bool IsImageFile(IFormFile file)
        {
            // 定义有效的图片 MIME 类型
            string[] allowedImageTypes = { "image/jpeg", "image/png", "image/gif" };

            // 检查文件类型是否为图片
            return allowedImageTypes.Contains(file.ContentType.ToLower());
        }



        // GET: Beautician/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var beautician = await _context.Beauticians.FindAsync(id);
            if (beautician == null)
            {
                return NotFound();
            }



            //beautician = await _context.Beautician.FirstOrDefaultAsync(b => b.id == id);
            int? loginID = HttpContext.Session.GetInt32("loginID");
            var Beauticians = _context.businesses
          .Where(b => b.ID == loginID)
          .ToList();

            ViewData["business_ID"] = new SelectList(Beauticians, "ID", "name");
            //ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", beautician.business_ID);
            return View(beautician);
        }

        // POST: Beautician/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,name,gender,photo,phone,business_ID,Highest_license,Remark")] Beautician beautician)
        {
            if (id != beautician.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 取得原始的 Beautician 資料
                    var originalBeautician = await _context.Beauticians.AsNoTracking().FirstOrDefaultAsync(b => b.id == beautician.id);

                    if (beautician.photo == null)
                    {
                        // 沒有新圖片上傳，保留原有的 photo 和 Highest_license 值
                        beautician.photo = originalBeautician.photo;
                    }
                    if (beautician.Highest_license == null)
                    {
                        beautician.Highest_license = originalBeautician.Highest_license;
                    }

                    // 檢查是否有新的圖片文件上傳
                    if (HttpContext.Request.Form.Files.Count > 0)
                    {
                        foreach (var file in HttpContext.Request.Form.Files)
                        {
                            if (file != null && file.Length > 0) // 檢查文件有效性
                            {
                                // 生成唯一的文件名，避免重复
                                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);

                                // 指定文件的完整路徑，保存到 wwwroot/Salon_img 文件夾下
                                string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "Salon_img", uniqueFileName);

                                // 保存文件到目標路徑
                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                }

                                // 根据文件的类型更新对应的属性
                                if (file.Name == "photo")
                                {
                                    // 刪除舊的檔案
                                    DeleteImageFile(originalBeautician.photo);

                                    // 更新 photo 屬性為新的文件名
                                    beautician.photo = uniqueFileName;
                                }
                                else if (file.Name == "Highest_license")
                                {
                                    // 刪除舊的檔案
                                    DeleteImageFile(originalBeautician.Highest_license);

                                    // 更新 Highest_license 屬性為新的文件名
                                    beautician.Highest_license = uniqueFileName;
                                }
                            }
                        }
                    }
                   

                    // 更新 Beautician 資料
                    _context.Update(beautician);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BeauticianExists(beautician.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                int? loginID2 = HttpContext.Session.GetInt32("loginID");
                var Beauticians2 = _context.businesses
           .Where(b => b.ID == loginID2)
           .ToList();

                ViewData["business_ID"] = new SelectList(Beauticians2, "ID", "name");
                // ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", beautician.business_ID);
                return RedirectToAction(nameof(Index));
            }
            int? loginID = HttpContext.Session.GetInt32("loginID");
            var Beauticians = _context.businesses
          .Where(b => b.ID == loginID)
          .ToList();

            ViewData["business_ID"] = new SelectList(Beauticians, "ID", "name");
            // ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", beautician.business_ID);
            return View(beautician);



       
        }

        private void DeleteImageFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "Salon_img", fileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }



        // GET: Beautician/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var beautician = await _context.Beauticians
                .Include(b => b.business)
                .FirstOrDefaultAsync(m => m.id == id);
            if (beautician == null)
            {
                return NotFound();
            }

            return View(beautician);
        }

        // POST: Beautician/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var beautician = await _context.Beauticians.FindAsync(id);
            if (beautician != null)
            {
                _context.Beauticians.Remove(beautician);
            }

            try
            {
                if (!string.IsNullOrEmpty(beautician.photo))
                {
                    // 指定第一張圖片的完整路徑
                    string imagePath1 = Path.Combine(_hostingEnvironment.WebRootPath, "Salon_img", beautician.photo);

                    // 刪除第一張圖片檔案
                    if (System.IO.File.Exists(imagePath1))
                    {
                        System.IO.File.Delete(imagePath1);
                    }
                }

                if (!string.IsNullOrEmpty(beautician.Highest_license))
                {
                    // 指定第二張圖片的完整路徑
                    string imagePath2 = Path.Combine(_hostingEnvironment.WebRootPath, "Salon_img", beautician.Highest_license);

                    // 刪除第二張圖片檔案
                    if (System.IO.File.Exists(imagePath2))
                    {
                        System.IO.File.Delete(imagePath2);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting image file: {ex.Message}");
            }



            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BeauticianExists(int id)
        {
            return _context.Beauticians.Any(e => e.id == id);
        }
    }
}
