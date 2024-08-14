using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using CRUD_COREMVC;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Tailstale.Models;

namespace Tailstale.Controllers
{
    [IsLoginFilter]
    public class Sbusiness_imgController : Controller
    {
        private readonly TailstaleContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public Sbusiness_imgController(TailstaleContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: business_img
        public async Task<IActionResult> Index()
        {
            int? loginID = HttpContext.Session.GetInt32("loginID");
            int? loginType = HttpContext.Session.GetInt32("loginType");
            var business_img_type = await _context.business_img_types
           .Where(b => b.FK_business_id == loginID)
           .ToListAsync();
            ViewData["img_type"] = new SelectList(business_img_type, "ID", "typename");
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Index(int? id)
        {



            // 準備查詢
            IQueryable<business_img> query = _context.business_imgs
                .Include(bh => bh.img_type)
                 .OrderByDescending(bh => bh.ID);


            // 根據 id 的情況添加條件
            if (id.HasValue)
            {
                query = query.Where(bh => bh.img_type_id == id);
            }



            // 执行查询并返回结果
            var S_img_type = await query.ToListAsync();

            return PartialView("_Sbusiness_imgPartial", S_img_type);


        }








        // GET: business_img/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var business_img = await _context.business_imgs
                .Include(b => b.img_type)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (business_img == null)
            {
                return NotFound();
            }

            return View(business_img);
        }

        // GET: business_img/Create
        public IActionResult Create()
        {
            int? loginID = HttpContext.Session.GetInt32("loginID");
            var query = from business in _context.businesses
                        join imgType in _context.business_img_types
                        on business.ID equals imgType.FK_business_id
                        where business.ID == loginID
                        select new
                        {
                            ImageTypeID = imgType.ID,
                            ImageTypeName = imgType.typename
                        };

            var result =  query.ToList();

            ViewData["ImageTypeList"] = new SelectList(result, "ImageTypeID", "ImageTypeName");
            ViewData["img_type_id"] = new SelectList(_context.business_img_types, "ID", "typename");
            return View();
        }

        // POST: business_img/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,img_type_id,URL,name,created_at")] Sbusiness_imgViewModel business_img)
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
                        business_img.URL = uniqueFileName;
                    }
                }
                business_img a = new business_img()
                {
                    img_type_id = business_img.img_type_id,
                    URL = business_img.URL,
                    name = business_img.name,
                };
                // 将 service 对象添加到数据库上下文并保存更改
                _context.Add(a);
                await _context.SaveChangesAsync();

                // 成功保存后重定向到 Index 页面
                return RedirectToAction(nameof(Index));
            }
            int? loginID = HttpContext.Session.GetInt32("loginID");
            var query = from business in _context.businesses
                        join imgType in _context.business_img_types
                        on business.ID equals imgType.FK_business_id
                        where business.ID == loginID
                        select new
                        {
                            ImageTypeID = imgType.ID,
                            ImageTypeName = imgType.typename
                        };

            var result = query.ToList();

            ViewData["ImageTypeList"] = new SelectList(result, "ImageTypeID", "ImageTypeName");


            //ViewData["img_type_id"] = new SelectList(_context.business_img_types, "ID", "ID", business_img.img_type_id);
            ViewData["img_type_id"] = new SelectList(_context.business_img_types, "ID", "typename", business_img.img_type_id);//預設選到我傳進來的這參數他的img_type_id
            return View(business_img);
        }

        // GET: business_img/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var business_img = await _context.business_imgs.FindAsync(id);
            if (business_img == null)
            {
                return NotFound();
            }
            int? loginID = HttpContext.Session.GetInt32("loginID");
            var query = from business in _context.businesses
                        join imgType in _context.business_img_types
                        on business.ID equals imgType.FK_business_id
                        where business.ID == loginID
                        select new
                        {
                            ImageTypeID = imgType.ID,
                            ImageTypeName = imgType.typename
                        };

            var result = query.ToList();

            ViewData["ImageTypeList"] = new SelectList(result, "ImageTypeID", "ImageTypeName");

            ViewData["img_type_id"] = new SelectList(_context.business_img_types, "ID", "typename", business_img.img_type_id);
            return View(business_img);
        }

        // POST: business_img/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,img_type_id,URL,name,created_at")] business_img business_img)
        {
            if (id != business_img.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 取得原始的 Service 資料
                    var originalService = await _context.business_imgs.AsNoTracking().FirstOrDefaultAsync(s => s.ID == business_img.ID);

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
                            DeleteImageFile(originalService.URL);

                            // 更新 service_img 屬性為新的文件名
                            business_img.URL = uniqueFileName;
                        }
                    }
                    else
                    {
                        // 沒有新圖片上傳，保留原有的 service_img 值
                        business_img.URL = originalService.URL;
                    }

                    // 更新 Service 資料
                    _context.Update(business_img);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!business_imgExists(business_img.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                int? loginID2 = HttpContext.Session.GetInt32("loginID");
                var query2 = from business in _context.businesses
                            join imgType in _context.business_img_types
                            on business.ID equals imgType.FK_business_id
                            where business.ID == loginID2
                             select new
                            {
                                ImageTypeID = imgType.ID,
                                ImageTypeName = imgType.typename
                            };

                var result2 = query2.ToList();

                ViewData["ImageTypeList"] = new SelectList(result2, "ImageTypeID", "ImageTypeName");


                ViewData["img_type_id"] = new SelectList(_context.business_img_types, "ID", "typename", business_img.img_type_id);
                return RedirectToAction(nameof(Index));
            }
            int? loginID = HttpContext.Session.GetInt32("loginID");
            var query = from business in _context.businesses
                        join imgType in _context.business_img_types
                        on business.ID equals imgType.FK_business_id
                        where business.ID == loginID
                        select new
                        {
                            ImageTypeID = imgType.ID,
                            ImageTypeName = imgType.typename
                        };

            var result = query.ToList();

            ViewData["ImageTypeList"] = new SelectList(result, "ImageTypeID", "ImageTypeName");


            ViewData["img_type_id"] = new SelectList(_context.business_img_types, "ID", "typename", business_img.img_type_id);
            return View(business_img);
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




        // GET: business_img/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var business_img = await _context.business_imgs
                .Include(b => b.img_type)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (business_img == null)
            {
                return NotFound();
            }

            return View(business_img);
        }

        // POST: business_img/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var business_img = await _context.business_imgs.FindAsync(id);
            if (business_img != null)
            {
                _context.business_imgs.Remove(business_img);
            }
            if (!string.IsNullOrEmpty(business_img.URL))
            {
                // 指定圖片的完整路徑
                string imagePath = Path.Combine(_hostingEnvironment.WebRootPath, "Salon_img", business_img.URL);

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

        private bool business_imgExists(int id)
        {
            return _context.business_imgs.Any(e => e.ID == id);
        }
    }
}
