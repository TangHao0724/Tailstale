using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System;
using System.Drawing;
using Tailstale.Models;
using Tailstale.Salon_DTO;

namespace Tailstale.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalonApiController : ControllerBase
    {

        TailstaleContext _context;

        public SalonApiController(TailstaleContext context)
        {
            _context = context;
        }

        // GET: api/SalonApi/GetService
        [HttpGet("GetService")]
        public async Task<IEnumerable<ServiceDTO>> GetService()
        {
            return _context.Services.Select(Emp => new ServiceDTO
            {
                id = Emp.id,
                business_ID = Emp.business_ID,
                category = Emp.category,
                service_name = Emp.service_name,
                service_content = Emp.service_content,
                service_img = Emp.service_img,
                price = Emp.price,
                created_at = Emp.created_at.HasValue ? Emp.created_at.Value.ToString("o") : null,
            });
        }

        // GET: api/SalonApi/GetSalonbusiness
        [HttpGet("GetSalonbusiness")]
        public async Task<IEnumerable<SalonbusinessDTO>> GetSalonbusiness(int id)
        {
            return _context.businesses
                .Where(Emp => Emp.ID == id)
                .Select(Emp => new SalonbusinessDTO
                {
                    ID = Emp.ID,
                    type_ID = Emp.type_ID,
                    name = Emp.name,
                    email = Emp.email,
                    phone = Emp.phone,
                    address = Emp.address,
                    geoJson = Emp.geoJson,
                    license_number = Emp.license_number,
                    business_status = Emp.business_status,
                    description = Emp.description,
                    photo_url = Emp.photo_url,
                    created_at = Emp.created_at.HasValue ? Emp.created_at.Value.ToString("o") : null,
                });
        }


        // GET: api/SalonApi/GetBusinessHour
        [HttpGet("GetBusinessHour")]
        public async Task<IEnumerable<BusinessHourDTO>> GetBusinessHour(int id)
        {
            return _context.Business_hours
                .Where(Emp => Emp.business_ID == id)
                .Select(Emp => new BusinessHourDTO
            {
                id = Emp.id,
                business_ID = Emp.business_ID,
                business_day = Emp.business_day.ToString("yyyy-MM-dd"),
                open_time = Emp.open_time.HasValue ? Emp.open_time.Value.ToString("HH:mm") : null,
                close_time = Emp.close_time.HasValue ? Emp.close_time.Value.ToString("HH:mm") : null,
                people_limit = Emp.people_limit,
            });
        }

        // GET: api/SalonApi/GetTimeRange
        [HttpGet("GetTimeRange")]
        public async Task<IActionResult> GetTimeRange(string day,int id)
        {
           
            if (!DateOnly.TryParse(day, out var dayDate))
            {
                Console.WriteLine($"Failed to parse day: {day}");
                return BadRequest(new { error = "Invalid date format. Expected format: yyyy-MM-dd" });
            }

            // 查詢對應的資料
            var businessHour = await _context.Business_hours
                .Where(bh => bh.business_day == dayDate && bh.business_ID == id)
                .FirstOrDefaultAsync();

            if (businessHour == null)
            {
                return NotFound("No data found for the specified day.");
            }

            // 確保時間格式有效
            if (businessHour.open_time == null || businessHour.close_time == null)
            {
                return StatusCode(500, new { error = "Open time or close time is null in database." });
            }

            // 生成時間範圍
            var timeList = new List<string>();
            var currentTime = businessHour.open_time.Value;
            var endTime = businessHour.close_time.Value;

            while (currentTime <= endTime)
            {
                //timeList.Add(currentTime.ToString(@"hh\:mm"));//12小時制
                timeList.Add(currentTime.ToString("HH:mm"));
                currentTime = currentTime.AddHours(1);
            }

            return Ok(timeList);
        }




        // GET: api/SalonApi/GetSalonPicture
        [HttpGet("GetSalonPicture")]
        public async Task<IEnumerable<Sbusiness_imgDTO>> GetSalonPicture()
        {
            return _context.business_imgs
                .Include(Emp => Emp.img_type)
                .Include(Emp => Emp.img_type.FK_business)
                .Select(Emp => new Sbusiness_imgDTO
                {
                    ID = Emp.ID,
                    img_type_id = Emp.img_type_id,
                    URL = Emp.URL,
                    name = Emp.name,
                    FK_business_id = Emp.img_type.FK_business_id,
                    business_name = Emp.img_type.FK_business.name,
                    typename = Emp.img_type.typename,
                    created_at = Emp.created_at.HasValue ? Emp.created_at.Value.ToString("o") : null,
                });
        }


        // Post: api/SalonApi/ReserveCreate
        [HttpPost("ReserveCreate")]
        public string ReserveCreate([FromForm] Reserve reserve)
        {
            if (reserve.keeper_id == 0 || string.IsNullOrEmpty(reserve.pet_name))
            {
                return "預約失敗,請先登入!";
            }


            _context.Reserves.Add(reserve);
            _context.SaveChangesAsync();

            return "預約完成!";
        }

        [HttpGet("SelectKeeperId")]
        public int? SelectKeeperId(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;

            }
            var Keeper = _context.keepers
            .Where(b => b.name == name)
            .Select(b => b.ID) // 假設 ID 是你需要的字段
              .FirstOrDefault(); // 返回第一筆符合條件的資料，如果找不到則返回預設值 (0)

            return Keeper == 0 ? (int?)null : Keeper;
        }

        // Get: api/SalonApi/SelectPetName
        [HttpGet("SelectPetName")]
        public IActionResult SelectPetName(int keeperid)
        {
            if (keeperid == null)
            {
                // 如果 keeperid 無效，返回空的 JSON 陣列
                return Ok(new List<object>());
            }

            var Keeper = _context.pets
         .Where(b => b.keeper_ID == keeperid)
         .Select(b => new
         {
             name = b.name // 假設你的模型中的 name 欄位名稱是 name
         })
         .ToList(); // 將結果轉換為 List

            return Ok(Keeper);

        }


        [HttpGet("SelectStore")]
        public IActionResult SelectStore(string address, string serve)
        {
            if (address == null || serve == null)
            {
                // 如果 keeperid 無效，返回空的 JSON 陣列
                return Ok(new List<object>());
            }

         var Keeper = _context.businesses
         .Include(b => b.Services)
         .Where(b => (b.address.Contains(address)) && b.Services.Any(s => s.service_name.Contains(serve)))
         .Select(b => new
         {
             ID = b.ID,             
             name = b.name,
             email = b.email,
             phone = b.phone,
             address = b.address,
             photo_url = b.photo_url,
         })
         .ToList(); // 將結果轉換為 List

            return Ok(Keeper);

        }





    }
}
