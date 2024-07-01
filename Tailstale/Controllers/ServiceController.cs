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
    public class ServiceController : Controller
    {
        private readonly TailstaleContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ServiceController(TailstaleContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: Service
        public async Task<IActionResult> Index()
        {
            
            var tailstaleContext = _context.Service.Include(s => s.business);
            return View(await tailstaleContext.ToListAsync());
        }

        // GET: Service/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Service
                .Include(s => s.business)
                .FirstOrDefaultAsync(m => m.id == id);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        // GET: Service/Create
        public IActionResult Create()
        {
            ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name");
            return View();
        }

        // POST: Service/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,business_ID,category,service_name,service_content,service_img,price")] Service service)
        {
            if (ModelState.IsValid)
            {
                if (HttpContext.Request.Form.Files.Count > 0) // 检查是否有文件上传
                {
                    var photoFile = HttpContext.Request.Form.Files[0]; // 获取第一个上传的文件

                    if (photoFile != null && photoFile.Length > 0) // 检查文件是否有效
                    {
                        // 生成唯一的文件名，避免重复
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(photoFile.FileName);

                        // 指定文件的完整路径，将文件保存到 wwwroot/images 文件夹下
                        string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "Salon_img", uniqueFileName);
                        

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await photoFile.CopyToAsync(stream); // 将文件复制到指定路径
                        }

                        // 将文件名（包含扩展名）保存到 service 对象的 service_img 属性
                        service.service_img = uniqueFileName;
                    }
                }

                // 将 service 对象添加到数据库上下文并保存更改
                _context.Add(service);
                await _context.SaveChangesAsync();

                // 成功保存后重定向到 Index 页面
                return RedirectToAction(nameof(Index));
            }
ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", service.business_ID);
            // 如果模型状态无效，返回 Create 页面以显示错误信息和重新填写表单
            return View(service);
            //if (ModelState.IsValid)
            //{
            //    _context.Add(service);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}
            //避免ModelState.IsValid 返回 false時,ViewData失效
            //return View(service);
        }

        // GET: Service/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Service.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }
            ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", service.business_ID);
            return View(service);
        }

        // POST: Service/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,business_ID,category,service_name,service_content,service_img,price")] Service service)
        {
            if (id != service.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 取得原始的 Service 資料
                    var originalService = await _context.Service.AsNoTracking().FirstOrDefaultAsync(s => s.id == service.id);

                    // 檢查是否有新的圖片文件上傳
                    if (HttpContext.Request.Form.Files.Count > 0)
                    {
                        var photoFile = HttpContext.Request.Form.Files[0]; // 取得第一個上傳的文件

                        if (photoFile != null && photoFile.Length > 0) // 檢查文件有效性
                        {
                            // 生成唯一的文件名，避免重复
                            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(photoFile.FileName);

                            // 指定文件的完整路徑，保存到 wwwroot/Salon_img 文件夾下
                            string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "Salon_img", uniqueFileName);

                            // 保存文件到目標路徑
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await photoFile.CopyToAsync(stream);
                            }

                            // 刪除舊的檔案
                            DeleteImageFile(originalService.service_img);

                            // 更新 service_img 屬性為新的文件名
                            service.service_img = uniqueFileName;
                        }
                    }
                    else
                    {
                        // 沒有新圖片上傳，保留原有的 service_img 值
                        service.service_img = originalService.service_img;
                    }

                    // 更新 Service 資料
                    _context.Update(service);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceExists(service.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", service.business_ID);
                return RedirectToAction(nameof(Index));
            }

            
            ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", service.business_ID);
            return View(service);
        }


        private void DeleteImageFile(string fileName)
        {
            //string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "Salon_img", fileName);
            //if (System.IO.File.Exists(filePath))
            //{
            //    System.IO.File.Delete(filePath);
            //}
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




        // GET: Service/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Service
                .Include(s => s.business)
                .FirstOrDefaultAsync(m => m.id == id);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        // POST: Service/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var service = await _context.Service.FindAsync(id);
            if (service != null)
            {
                _context.Service.Remove(service);
            }
            if (!string.IsNullOrEmpty(service.service_img))
            {
                // 指定圖片的完整路徑
                string imagePath = Path.Combine(_hostingEnvironment.WebRootPath, "Salon_img", service.service_img);

                try
                {
                    // 刪除圖片檔案
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }
                catch (Exception ex)
                {
                    
                    throw new Exception($"Error deleting image file: {ex.Message}");
                }
            }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            
        }

        private bool ServiceExists(int id)
        {
            return _context.Service.Any(e => e.id == id);
        }
    }
}
