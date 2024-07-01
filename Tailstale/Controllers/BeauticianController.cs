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
            var tailstaleContext = _context.Beautician.Include(b => b.business);
            return View(await tailstaleContext.ToListAsync());
        }

        // GET: Beautician/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var beautician = await _context.Beautician
                .Include(b => b.business)
                .FirstOrDefaultAsync(m => m.id == id);
            if (beautician == null)
            {
                return NotFound();
            }

            return View(beautician);
        }

        // GET: Beautician/Create
        public IActionResult Create()
        {
            ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name");
            return View();
        }

        // POST: Beautician/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,name,gender,photo,phone,business_ID,Highest_license,Remark")] Beautician beautician)
        {
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
                            ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", beautician.business_ID);
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
                            ModelState.AddModelError("Highest_license", "Only image files are allowed for Highest License.");
                            ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", beautician.business_ID);
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

                // 将 beautician 对象添加到数据库上下文并保存更改
                _context.Add(beautician);
                await _context.SaveChangesAsync();
                ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", beautician.business_ID);
                // 成功保存后重定向到 Index 页面
                return RedirectToAction(nameof(Index));
            }

            ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", beautician.business_ID);
            return View(beautician);



            //if (ModelState.IsValid)
            //{
            //    _context.Add(beautician);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}
            //ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", beautician.business_ID);
            //return View(beautician);
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

            var beautician = await _context.Beautician.FindAsync(id);
            if (beautician == null)
            {
                return NotFound();
            }
            ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", beautician.business_ID);
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", beautician.business_ID);
            return View(beautician);
        }

        // GET: Beautician/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var beautician = await _context.Beautician
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
            var beautician = await _context.Beautician.FindAsync(id);
            if (beautician != null)
            {
                _context.Beautician.Remove(beautician);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BeauticianExists(int id)
        {
            return _context.Beautician.Any(e => e.id == id);
        }
    }
}
