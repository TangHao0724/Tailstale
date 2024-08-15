using CRUD_COREMVC;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using System.Drawing;
using Tailstale.Models;


namespace Tailstale.Controllers
{
   
    public class SalonController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<SalonController> _logger;
        private readonly TailstaleContext _context;

        //  private readonly TailstaleContext _context;

        public SalonController(ILogger<SalonController> logger, TailstaleContext context, IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        [IsLoginFilter]
        public IActionResult Settings()
        {
            return View();
        }


        [IsLoginFilter]
        public IActionResult SalonLog()
        {

            return View();
        }

        public IActionResult SalonHome()
        {
            int? KloginID = HttpContext.Session.GetInt32("KloginID");
            int? loginID = HttpContext.Session.GetInt32("loginID");
            int? loginType = HttpContext.Session.GetInt32("loginType");

            string KloginName = HttpContext.Session.GetString("KloginName");

            var addresses = _context.businesses
           .Where(b => b.type_ID == 2 && b.address.Length >= 4)
           .Select(b => b.address)
           .ToList();

            ViewBag.Addresses = addresses;



            // 將值設置到 ViewBag
            ViewBag.LoginName = KloginName;


            ViewBag.KLoginID = KloginID.HasValue ? KloginID.Value : (int?)null;
            ViewBag.LoginID = loginID.HasValue ? loginID.Value : (int?)null;
            ViewBag.LoginType = loginType.HasValue ? loginType.Value : (int?)null;
            ViewData["keeper_ID"] = new SelectList(_context.keepers, "ID", "name");

            if (KloginID.HasValue)  
            {
                // loginID 有值，進行查詢
                var pets = _context.pets
                                   .Where(p => p.keeper_ID == KloginID.Value)
                                   .ToList();

                if (pets.Any())
                {
                    // 找到匹配的資料

                    ViewData["pet_name"] = new SelectList(pets, "name", "name");
                }
                else
                {
                    //ViewData["pet_name"] = new SelectList(Enumerable.Empty<SelectListItem>());//可有可無
                }
            }


            return View();
        }

        [IsLoginFilter]
        public async Task<IActionResult> businessEdit()
        {

            int? loginID = HttpContext.Session.GetInt32("loginID");
            ViewBag.LoginID = loginID.HasValue ? loginID.Value : (int?)null;


            if (loginID == null)
            {
                return NotFound();
            }

            var businesses = await _context.businesses.FindAsync(loginID);
            if (businesses == null)
            {
                return NotFound();
            }


            return View(businesses);
        }

        // POST: Reserves/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> businessEdit(int id, [Bind("ID,password,salt,type_ID,name,email,phone,address,geoJson,license_number,business_status,description,photo_url,created_at")] business business)
        {
            if (id != business.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 取得原始的 Service 資料
                    var originalService = await _context.businesses.AsNoTracking().FirstOrDefaultAsync(s => s.ID == business.ID);

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
                            DeleteImageFile(originalService.photo_url);

                            // 更新 service_img 屬性為新的文件名
                            business.photo_url = uniqueFileName;
                        }
                    }
                    else
                    {
                        // 沒有新圖片上傳，保留原有的 service_img 值
                        business.photo_url = originalService.photo_url;
                    }
                    var existingbusinesses = await _context.businesses.FindAsync(id);

                    existingbusinesses.name = business.name;
                    existingbusinesses.phone = business.phone;
                    existingbusinesses.address = business.address;
                    existingbusinesses.geoJson = business.geoJson;
                    existingbusinesses.license_number = business.license_number;
                    existingbusinesses.description = business.description;
                    existingbusinesses.photo_url = business.photo_url;

                    // 更新 Service 資料
                    _context.Update(existingbusinesses);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!business_Exists(business.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction("Index", "Home", new { area = "Home" });
            }
            return View(business);
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


        private bool business_Exists(int id)
        {
            return _context.business_imgs.Any(e => e.ID == id);
        }

    }
}
