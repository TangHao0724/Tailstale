using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tailstale.Hospital_ViewModel;
using Tailstale.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Tailstale.Controllers
{
    public class vet_informationController : Controller
    {
        private readonly TailstaleContext _context;

        public vet_informationController(TailstaleContext context)
        {
            _context = context;            
        }

        // GET: vet_information
        public async Task<IActionResult> Index()
        {
            try
            {
                var tailstaleContext = _context.vet_informations.Include(v => v.business).Include(v => v.department);

                var v_Infovm = from v_info in _context.vet_informations
                               join b_img in _context.business_imgs
                               on v_info.business_img_ID equals b_img.ID
                               join b_img_types in _context.business_img_types
                               on b_img.img_type_id equals b_img_types.ID
                               orderby v_info.employment_status descending
                               select new vet_information_ViewModel
                               {
                                   vet_ID = v_info.vet_ID,
                                   vet_name = v_info.vet_name,
                                   license_number = v_info.license_number,
                                   profile = v_info.profile,
                                   business_ID = v_info.business_ID,
                                   business = v_info.business,
                                   department_ID = v_info.department_ID,
                                   department = v_info.department,
                                   business_img_ID = b_img.ID,
                                   business_img= b_img,
                                   img_type_id = b_img_types.ID,
                                   URL = b_img.URL,
                                   name = b_img.name,
                                   employment_status=v_info.employment_status,
                               };

                return View(v_Infovm);
            }
            catch (Exception ex)
            {
                // 記錄錯誤訊息
                // Log.Error(ex, "An error occurred while executing Index.");
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        // GET: vet_information/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var v_Infovm = await (from v_info in _context.vet_informations
                                  join b_img in _context.business_imgs
                                  on v_info.business_img_ID equals b_img.ID
                                  join b_img_types in _context.business_img_types
                                  on b_img.img_type_id equals b_img_types.ID
                                  where v_info.vet_ID == id
                                  select new vet_information_ViewModel
                                  {
                                      vet_ID = v_info.vet_ID,
                                      vet_name = v_info.vet_name,
                                      license_number = v_info.license_number,
                                      profile = v_info.profile,
                                      business_ID = v_info.business_ID,
                                      business = v_info.business,
                                      department_ID = v_info.department_ID,
                                      department = v_info.department,
                                      business_img_ID = b_img.ID,
                                      business_img = b_img,
                                      img_type_id = b_img_types.ID,
                                      URL = b_img.URL,
                                      name = b_img.name,
                                      employment_status=v_info.employment_status,
                                  }).FirstOrDefaultAsync();

            return View(v_Infovm);
        }

        // GET: vet_information/Create
        public IActionResult Create(int id)
        {
            ViewBag.typename = new SelectList(_context.business_img_types.Where(t=>t.FK_business_id==id), "ID", "typename");
            ViewData["business_ID"] = new SelectList(_context.businesses.Where(b=>b.ID==id), "ID", "name");
            ViewData["department_ID"] = new SelectList(_context.departments.Where(d=>d.business_ID==id), "department_ID", "department_name");
            return View();
        }


        // POST: vet_information/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(insert_vet_information_ViewModel v_Infovm/*, IFormFile? file*/)
        {
            if (ModelState.IsValid)
            {                
                // 判斷是否有上傳檔案
                if (Request.Form.Files["URL"] != null)
                {
                    // 取得照片欄位名稱
                    var pictureFile = Request.Form.Files["URL"];

                    // 新增存圖檔路徑
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/lib/HospitalImages/Vet_Info");
                    // 確保目標目錄存在
                    if (!Directory.Exists(uploadsFolder))
                    {
                        // 如果路徑不在則創建
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // 生成唯一的文件名以避免重名
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + pictureFile.FileName;

                    // 目標文件的完整路徑
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // 將文件保存到指定路徑
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await pictureFile.CopyToAsync(fileStream);
                    }
                    // 更新圖片路徑
                    v_Infovm.URL = uniqueFileName;
                    //v_Infovm.name = uniqueFileName;
                }

                business_img business_Img = new business_img
                {
                    URL = v_Infovm.URL,
                    //name = v_Infovm.name,
                    name = v_Infovm.URL,
                    img_type_id=v_Infovm.img_type_id,
                };
                _context.Add(business_Img);
                await _context.SaveChangesAsync();

                //vet_Information vet_Information = new vet_Information
                //{
                //    vet_name = v_Infovm.vet_name,
                //    business_ID = v_Infovm.business_ID,
                //    license_number = v_Infovm.license_number,
                //    department_ID = v_Infovm.department_ID,
                //    profile = v_Infovm.profile,
                //    business_img_ID = business_Img.ID,
                //    employment_status=v_Infovm.employment_status,
                //};
                //_context.Add(vet_Information);
                await _context.SaveChangesAsync();
            }
            else
            {
                return View(v_Infovm);
            }
            ViewBag.typename= new SelectList(_context.business_img_types, "ID","typename");
            ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", v_Infovm.business_ID);
            ViewData["department_ID"] = new SelectList(_context.departments, "department_ID", "department_name", v_Infovm.department_ID);
            return RedirectToAction(nameof(Index));
        }

        // GET: vet_information/Edit
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }
            var v_Infovm = await(from v_info in _context.vet_informations
                           join b_img in _context.business_imgs
                           on v_info.business_img_ID equals b_img.ID
                           join b_img_types in _context.business_img_types
                           on b_img.img_type_id equals b_img_types.ID
                           where v_info.vet_ID== id
                           select new vet_information_ViewModel
                           {
                               vet_ID = v_info.vet_ID,
                               vet_name = v_info.vet_name,
                               license_number = v_info.license_number,
                               profile = v_info.profile,
                               business_ID = v_info.business_ID,
                               business = v_info.business,
                               department_ID = v_info.department_ID,
                               department = v_info.department,
                               business_img_ID = b_img.ID,
                               business_img = b_img,
                               img_type_id = b_img_types.ID,
                               URL = b_img.URL,
                               name = b_img.name,
                               employment_status=v_info.employment_status,
                           }).FirstOrDefaultAsync();

            ViewData["business_ID"] = new SelectList(_context.businesses.Where(b=>b.type_ID==3), "ID", "name");
            ViewData["department_ID"] = new SelectList(_context.departments, "department_ID", "department_name", v_Infovm.department_ID);

            return View(v_Infovm);
        }

        // POST: vet_information/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("vet_ID,vet_name,business_ID,license_number,department_ID,business_img_ID,profile,ID,img_type_id,URL,name,employment_status")] vet_information_ViewModel v_Infovm)
        {
            if (id != v_Infovm.vet_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 取出原先所有資料                    
                    var b_img = await _context.business_imgs.FindAsync(v_Infovm.business_img_ID);
                    // 判斷是否有上傳檔案
                    if (Request.Form.Files["URL"] != null)
                    {
                        // 取得照片欄位名稱
                        var pictureFile = Request.Form.Files["URL"];

                        // 新增存圖檔路徑
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/lib/HospitalImages/Vet_Info");
                        // 確保目標目錄存在
                        if (!Directory.Exists(uploadsFolder))
                        {
                            // 如果路徑不在則創建
                            Directory.CreateDirectory(uploadsFolder);
                        }
                        // 生成唯一的文件名以避免重名
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + pictureFile.FileName;

                        // 目標文件的完整路徑
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        // 將文件保存到指定路徑
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await pictureFile.CopyToAsync(fileStream);
                        }
                        // 更新圖片路徑
                        v_Infovm.URL =uniqueFileName;
                    }
                    else
                    {
                        // 放入原先資料
                        v_Infovm.URL = b_img.URL;
                    }
                    // 刪除舊圖片文件（如果有）
                    // 判斷是否原有圖片
                    if (Request.Form.Files["URL"] != null)
                    {
                        // 取得當前目錄,圖片存放路徑, 去掉路徑開頭的 / 符號，以防止路徑不正確
                        var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/lib/HospitalImages/Vet_Info", b_img.URL.TrimStart('/'));
                        // 檢查舊圖片文件是否存在
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            // 存在則刪除照片
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    // 解除追蹤
                    _context.Entry(b_img).State = EntityState.Detached;

                    business_img business_ImgUpdate = new business_img
                    {                        
                        ID= b_img.ID,
                        URL = v_Infovm.URL,
                        img_type_id = v_Infovm.img_type_id,
                        name = v_Infovm.URL,
                    };
                    _context.Update(business_ImgUpdate);
                    await _context.SaveChangesAsync();

                    //vet_Information vet_InformationUpdate = new vet_Information
                    //{
                    //    vet_ID=v_Infovm.vet_ID,
                    //    vet_name = v_Infovm.vet_name,
                    //    business_ID = v_Infovm.business_ID,
                    //    license_number = v_Infovm.license_number,
                    //    department_ID = v_Infovm.department_ID,
                    //    profile = v_Infovm.profile,
                    //    employment_status=v_Infovm.employment_status,
                    //    business_img_ID = business_ImgUpdate.ID,
                    //};
                    //_context.Update(vet_InformationUpdate);
                    await _context.SaveChangesAsync(); 
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!vet_informationExists(v_Infovm.vet_ID))
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
            ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", v_Infovm.business_ID);
            ViewData["department_ID"] = new SelectList(_context.departments, "department_ID", "department_name", v_Infovm.department_ID);
            
            return RedirectToAction(nameof(Index));
        }

        // GET: vet_information/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var v_Infovm = await (from v_info in _context.vet_informations
                                   join b_img in _context.business_imgs
                                   on v_info.business_img_ID equals b_img.ID
                                   join b_img_types in _context.business_img_types
                                   on b_img.img_type_id equals b_img_types.ID
                                   where v_info.vet_ID == id
                                   select new vet_information_ViewModel
                                   {
                                       vet_ID = v_info.vet_ID,
                                       vet_name = v_info.vet_name,
                                       license_number = v_info.license_number,
                                       profile = v_info.profile,
                                       business_ID = v_info.business_ID,
                                       business = v_info.business,
                                       department_ID = v_info.department_ID,
                                       department = v_info.department,
                                       business_img_ID = b_img.ID,
                                       business_img = b_img,
                                       img_type_id = b_img_types.ID,
                                       URL = b_img.URL,
                                       name = b_img.name
                                   }).FirstOrDefaultAsync();
            return View(v_Infovm);
        }

        // POST: vet_information/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vet_information = await _context.vet_informations.FindAsync(id);
            if (vet_information != null)
            {
                var business_img = await _context.business_imgs
                    .FirstOrDefaultAsync(b => b.ID == vet_information.business_img_ID);

                if (business_img != null)
                {
                    _context.vet_informations.Remove(vet_information);
                    _context.business_imgs.Remove(business_img);

                    if (!string.IsNullOrEmpty(business_img.URL))
                    {
                        // 取得當前目錄,圖片存放路徑, 去掉路徑開頭的 / 符號，以防止路徑不正確
                        var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/lib/HospitalImages/Vet_Info", business_img.URL.TrimStart('/'));
                        // 檢查舊圖片文件是否存在
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            // 存在則刪除照片
                            System.IO.File.Delete(oldFilePath);
                        }
                    }
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool vet_informationExists(int id)
        {
            return _context.vet_informations.Any(e => e.vet_ID == id);
        }
    }
}
