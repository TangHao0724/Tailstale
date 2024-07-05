using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tailstale.Models;

namespace Tailstale.Controllers
{
    public class Consumption_RecordController : Controller
    {
        private readonly TailstaleContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public Consumption_RecordController(TailstaleContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: Consumption_Record
        public async Task<IActionResult> Index()
        {
            var businesses = await _context.businesses
            .Where(b => b.type_ID == 2)
            .ToListAsync();

            ViewData["business_type"] = new SelectList(businesses, "type_ID", "name");
            return View();
            //var tailstaleContext = _context.Consumption_Record.Include(c => c.beautician).Include(c => c.business).Include(c => c.keeper);
            //return View(await tailstaleContext.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Index(int aid,int uid)
        {
            //var tailstaleContext = _context.Business_hours.Include(b => b.business);
            //return View(await tailstaleContext.ToListAsync());
            var business = await _context.businesses
        .FirstOrDefaultAsync(b => b.type_ID == aid);

            var business2 = await _context.Consumption_Record
        .FirstOrDefaultAsync(b => b.keeper_id == uid);

            if (business == null && business2 == null)
            {
                // 如果未找到符合条件的 business 记录，返回空的 PartialView
                return PartialView("_Consumption_RecordPartial", new List<Consumption_Record>());
            }

            // 查询符合条件的 Beautician 记录
            
            
            if (aid == 2)
            {
var beauticians = _context.Consumption_Record
                .Include(bh => bh.business)
                .Include(bh => bh.beautician)
                .Include(bh => bh.keeper)
                .Where(bh => bh.business_ID == business.ID)
                .ToList();
              return PartialView("_Consumption_RecordPartial", beauticians);
            }
            else
            {
var beauticians2 = _context.Consumption_Record
               .Where(bh => bh.keeper_id == business2.keeper_id)
               .Include(bh => bh.business)
               .Include(bh => bh.beautician)
                .Include(bh => bh.keeper)
               .ToList();
                return PartialView("_Consumption_RecordPartial", beauticians2);
            }
            // 返回到前端，假設你的 View 期望接收一段 HTML 作為結果
           
        }



        // GET: Consumption_Record/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consumption_Record = await _context.Consumption_Record
                .Include(c => c.beautician)
                .Include(c => c.business)
                .Include(c => c.keeper)
                .FirstOrDefaultAsync(m => m.id == id);
            if (consumption_Record == null)
            {
                return NotFound();
            }

            return View(consumption_Record);
        }

        // GET: Consumption_Record/Create
        public IActionResult Create()
        {
            var businesses = _context.businesses
            .Where(b => b.type_ID == 2)
            .ToList();

            ViewData["business_ID"] = new SelectList(businesses, "ID", "name");
            ViewData["beautician_id"] = new SelectList(_context.Beautician, "id", "name");
            //ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name");
            ViewData["keeper_id"] = new SelectList(_context.keepers, "ID", "name");
            return View();
        }

        // POST: Consumption_Record/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,keeper_id,pet_name,business_ID,time,beautician_id,service_name,pet_weight,price,end_time,before_photo,after_photo")] Consumption_Record consumption_Record)
        {
            var businesses = _context.businesses
            .Where(b => b.type_ID == 2)
            .ToList();
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
                            ModelState.AddModelError("photo", "Only image files are allowed for Photo.");
                            ViewData["beautician_id"] = new SelectList(_context.Beautician, "id", "name", consumption_Record.beautician_id);
                            ViewData["business_ID"] = new SelectList(businesses, "ID", "name");
                            //ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", consumption_Record.business_ID);
                            ViewData["keeper_id"] = new SelectList(_context.keepers, "ID", "name", consumption_Record.keeper_id);
                            return View(consumption_Record);
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
                        consumption_Record.after_photo = uniqueFileName;
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
                            ModelState.AddModelError("Highest_license", "Only image files are allowed for Highest License.");
                            ViewData["business_ID"] = new SelectList(businesses, "ID", "name");
                            ViewData["beautician_id"] = new SelectList(_context.Beautician, "id", "name", consumption_Record.beautician_id);
                            //ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", consumption_Record.business_ID);
                            ViewData["keeper_id"] = new SelectList(_context.keepers, "ID", "name", consumption_Record.keeper_id);
                            return View(consumption_Record);
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
                        consumption_Record.before_photo = uniqueFileName;
                    }
                }

                // 将 beautician 对象添加到数据库上下文并保存更改
                _context.Add(consumption_Record);
                await _context.SaveChangesAsync();
                

                ViewData["business_ID"] = new SelectList(businesses, "ID", "name");
                ViewData["beautician_id"] = new SelectList(_context.Beautician, "id", "name", consumption_Record.beautician_id);
                //ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", consumption_Record.business_ID);
                ViewData["keeper_id"] = new SelectList(_context.keepers, "ID", "name", consumption_Record.keeper_id);
                // 成功保存后重定向到 Index 页面
                return RedirectToAction(nameof(Index));
            }






           
            

            ViewData["business_ID"] = new SelectList(businesses, "ID", "name");
            ViewData["beautician_id"] = new SelectList(_context.Beautician, "id", "gender", consumption_Record.beautician_id);
            //ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", consumption_Record.business_ID);
            ViewData["keeper_id"] = new SelectList(_context.keepers, "ID", "address", consumption_Record.keeper_id);
            return View(consumption_Record);
        }


        private bool IsImageFile(IFormFile file)
        {
            // 定义有效的图片 MIME 类型
            string[] allowedImageTypes = { "image/jpeg", "image/png", "image/gif" };

            // 检查文件类型是否为图片
            return allowedImageTypes.Contains(file.ContentType.ToLower());
        }



        // GET: Consumption_Record/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consumption_Record = await _context.Consumption_Record.FindAsync(id);
            if (consumption_Record == null)
            {
                return NotFound();
            }
            var businesses = _context.businesses
            .Where(b => b.type_ID == 2)
            .ToList();

            ViewData["business_ID"] = new SelectList(businesses, "ID", "name");
            ViewData["beautician_id"] = new SelectList(_context.Beautician, "id", "name", consumption_Record.beautician_id);
            //ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", consumption_Record.business_ID);
            ViewData["keeper_id"] = new SelectList(_context.keepers, "ID", "name", consumption_Record.keeper_id);
            return View(consumption_Record);
        }

        // POST: Consumption_Record/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,keeper_id,pet_name,business_ID,time,beautician_id,service_name,pet_weight,price,end_time,before_photo,after_photo")] Consumption_Record consumption_Record)
        {
            if (id != consumption_Record.id)
            {
                return NotFound();
            }

            var businesses = _context.businesses
            .Where(b => b.type_ID == 2)
            .ToList();

            if (ModelState.IsValid)
            {
                try
                {
                    // 取得原始的 Beautician 資料
                    var originalConsumption_Record = await _context.Consumption_Record.AsNoTracking().FirstOrDefaultAsync(b => b.id == consumption_Record.id);

                    if (consumption_Record.after_photo == null) {
                        // 沒有新圖片上傳，保留原有的 photo 和 Highest_license 值
                       
                        consumption_Record.after_photo = originalConsumption_Record.after_photo;
                    }
                    if (consumption_Record.before_photo == null)
                    {
                        consumption_Record.before_photo = originalConsumption_Record.before_photo;
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
                                if (file.Name == "before_photo")
                                {
                                    // 刪除舊的檔案
                                    DeleteImageFile(originalConsumption_Record.before_photo);

                                    // 更新 photo 屬性為新的文件名
                                    consumption_Record.before_photo = uniqueFileName;
                                }
                                else if (file.Name == "after_photo")
                                {
                                    // 刪除舊的檔案
                                    DeleteImageFile(originalConsumption_Record.after_photo);

                                    // 更新 Highest_license 屬性為新的文件名
                                    consumption_Record.after_photo = uniqueFileName;
                                }
                            }
                        }
                    }
                    
                    
                        
                    

                    // 更新 Beautician 資料
                    _context.Update(consumption_Record);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Consumption_RecordExists(consumption_Record.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                

                ViewData["business_ID"] = new SelectList(businesses, "ID", "name");
                ViewData["beautician_id"] = new SelectList(_context.Beautician, "id", "name", consumption_Record.beautician_id);
                //ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", consumption_Record.business_ID);
                ViewData["keeper_id"] = new SelectList(_context.keepers, "ID", "name", consumption_Record.keeper_id);
                return RedirectToAction(nameof(Index));
            }



            

            ViewData["business_ID"] = new SelectList(businesses, "ID", "name");
            ViewData["beautician_id"] = new SelectList(_context.Beautician, "id", "name", consumption_Record.beautician_id);
            //ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", consumption_Record.business_ID);
            ViewData["keeper_id"] = new SelectList(_context.keepers, "ID", "name", consumption_Record.keeper_id);
            return View(consumption_Record);
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




        // GET: Consumption_Record/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consumption_Record = await _context.Consumption_Record
                .Include(c => c.beautician)
                .Include(c => c.business)
                .Include(c => c.keeper)
                .FirstOrDefaultAsync(m => m.id == id);
            if (consumption_Record == null)
            {
                return NotFound();
            }

            return View(consumption_Record);
        }

        // POST: Consumption_Record/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var consumption_Record = await _context.Consumption_Record.FindAsync(id);
            if (consumption_Record != null)
            {
                _context.Consumption_Record.Remove(consumption_Record);
            }


            try
            {
                if (!string.IsNullOrEmpty(consumption_Record.before_photo))
                {
                    // 指定第一張圖片的完整路徑
                    string imagePath1 = Path.Combine(_hostingEnvironment.WebRootPath, "Salon_img", consumption_Record.before_photo);

                    // 刪除第一張圖片檔案
                    if (System.IO.File.Exists(imagePath1))
                    {
                        System.IO.File.Delete(imagePath1);
                    }
                }

                if (!string.IsNullOrEmpty(consumption_Record.after_photo))
                {
                    // 指定第二張圖片的完整路徑
                    string imagePath2 = Path.Combine(_hostingEnvironment.WebRootPath, "Salon_img", consumption_Record.after_photo);

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



            //if (!string.IsNullOrEmpty(consumption_Record.photo))
            //{
            //    // 指定圖片的完整路徑
            //    string imagePath1 = Path.Combine(_hostingEnvironment.WebRootPath, "Salon_img", consumption_Record.before_photo);
            //    string imagePath2 = Path.Combine(_hostingEnvironment.WebRootPath, "Salon_img", consumption_Record.after_photo);
            //    try
            //    {
            //        // 刪除圖片檔案
            //        if (System.IO.File.Exists(imagePath1))
            //        {
            //            System.IO.File.Delete(imagePath1);
            //        }
            //        if (System.IO.File.Exists(imagePath2))
            //        {
            //            System.IO.File.Delete(imagePath2);
            //        }
            //    }
            //    catch (Exception ex)
            //    {

            //        throw new Exception($"Error deleting image file: {ex.Message}");
            //    }
            //}




            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Consumption_RecordExists(int id)
        {
            return _context.Consumption_Record.Any(e => e.id == id);
        }
    }
}
