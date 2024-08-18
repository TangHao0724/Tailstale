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
        public async Task<IActionResult> GetTimeRange(string day,int id, int people)
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

            var endTimePlusOneHour = endTime.AddHours(-1);

            while (currentTime <= endTimePlusOneHour)
            {
                // 檢查是否有足夠空位
                var dateTimeToCheck = dayDate.ToDateTime(currentTime);
                var reservationCount = await _context.Reserves
                    .Where(r => r.business_ID == id && (r.status == 5 || r.status == 6 || r.status == 8))
                    .CountAsync(r => r.time.Date == dateTimeToCheck.Date && r.time.TimeOfDay == dateTimeToCheck.TimeOfDay);

                //.CountAsync(r => r.business_ID == id && r.time.Date == dateTimeToCheck.Date && r.time.TimeOfDay == dateTimeToCheck.TimeOfDay);

                if (reservationCount < people)
                {
                    timeList.Add(currentTime.ToString("HH:mm"));
                }

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
        public async Task<string> ReserveCreate([FromForm] Reserve reserve)
        {
            if (reserve.keeper_id == 0 || string.IsNullOrEmpty(reserve.pet_name))
            {
                return "預約失敗,請先登入!";
            }

            try
            {
                _context.Reserves.Add(reserve);
                await _context.SaveChangesAsync();

                return "預約完成!";
            }
            catch (Exception ex)
            {
                // 记录异常日志（可选）
                // _logger.LogError(ex, "Error occurred while creating reservation");

                // 返回错误响应
                return "預約失敗,請重試";
            }

            //_context.Reserves.Add(reserve);
            //_context.SaveChangesAsync();

            //return "預約完成!";
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




        // GET: api/SalonApi/GetReserve
        [HttpGet("GetReserve")]
        public async Task<IEnumerable<ReserveDTO>> GetReserve()
        {
            int? KloginID = HttpContext.Session.GetInt32("KloginID");
            var businesses = await _context.businesses.ToListAsync();

            var query = from reserve in _context.Reserves
                        join keeper in _context.keepers on reserve.keeper_id equals keeper.ID
                        join business in _context.businesses on reserve.business_ID equals business.ID
                        join status in _context.order_statuses on reserve.status equals status.ID
                        where reserve.keeper_id == KloginID
                        orderby reserve.id descending // 按 id 降序排序
                        select new ReserveDTO
                        {
                            id = reserve.id,
                            keeper_id = reserve.keeper_id,
                            keeper_name = keeper.name,
                            pet_name = reserve.pet_name,
                            business_ID = reserve.business_ID,
                            business_name = business.name,
                            time = reserve.time.ToString("o"),
                            service_name = reserve.service_name,
                            status = reserve.status,
                            status_name = status.status_name,
                            created_at = reserve.created_at.HasValue ? reserve.created_at.Value.ToString("o") : null,
                        };

            return await query.ToListAsync();
        }


        // PUT: api/Reserve/CancelReservation/5
        [HttpPut("CancelReservation/{id}")]
        public async Task<IActionResult> CancelReservation(int id)
        {
            // 查找指定 ID 的 Reserve 項目
            var reserve = await _context.Reserves.FindAsync(id);

            if (reserve == null)
            {
                return NotFound(); // 如果找不到該項目，返回 404 Not Found
            }

            // 更新 status 欄位
            reserve.status = 10;

            // 保存更改到資料庫
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // 處理資料庫更新異常
                return StatusCode(500, "Internal server error. Could not update reservation.");
            }

            return NoContent(); // 返回 204 No Content 表示成功更新
        }

        // GET: api/SalonApi/GetConsumption
        [HttpGet("GetConsumption")]
        public async Task<IEnumerable<Consumption_RecordDTO>> GetConsumption()
        {
            int? KloginID = HttpContext.Session.GetInt32("KloginID");
            var businesses = await _context.businesses.ToListAsync();

            var query = from Consumption in _context.Consumption_Records
                        join keeper in _context.keepers on Consumption.keeper_id equals keeper.ID
                        join business in _context.businesses on Consumption.business_ID equals business.ID
                        join beautician in _context.Beauticians on Consumption.beautician_id equals beautician.id
                        where Consumption.keeper_id == KloginID
                        orderby Consumption.id descending // 按 id 降序排序
                        select new Consumption_RecordDTO
                        {
                            id = Consumption.id,
                            keeper_id = Consumption.keeper_id,
                            keeper_name = keeper.name,
                            pet_name = Consumption.pet_name,
                            business_ID = Consumption.business_ID,
                            business_name = business.name,
                            beautician_id = Consumption.beautician_id,
                            beautician_name = beautician.name,
                            time = Consumption.time.ToString("o"),
                            service_name = Consumption.service_name,
                            pet_weight = Consumption.pet_weight,
                            price = Consumption.price,
                            before_photo = Consumption.before_photo,
                            after_photo = Consumption.after_photo,
                            end_time = Consumption.end_time.HasValue ? Consumption.end_time.Value.ToString("o") : null,
                        };

            return await query.ToListAsync();
        }





    }
}
