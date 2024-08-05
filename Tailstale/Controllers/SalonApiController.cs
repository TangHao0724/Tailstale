using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IEnumerable<SalonbusinessDTO>> GetSalonbusiness()
        {
            return _context.businesses
                .Where(Emp => Emp.type_ID == 2)
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
        public async Task<IEnumerable<BusinessHourDTO>> GetBusinessHour()
        {
            return _context.Business_hours.Select(Emp => new BusinessHourDTO
            {
                id = Emp.id,
                business_ID = Emp.business_ID,
                business_day = Emp.business_day.ToString("yyyy-MM-dd"),
                open_time = Emp.open_time.HasValue ? Emp.open_time.Value.ToString("HH:mm") : null,
                close_time = Emp.close_time.HasValue ? Emp.close_time.Value.ToString("HH:mm") : null,
                people_limit = Emp.people_limit,
            });
        }

        // GET: api/SalonApi/GetSalonPicture
        [HttpGet("GetSalonPicture")]
        public async Task<IEnumerable<Sbusiness_imgDTO>> GetSalonPicture()
        {
            return _context.business_imgs.Select(Emp => new Sbusiness_imgDTO
            {
                ID = Emp.ID,
                img_type_id = Emp.img_type_id,
                URL = Emp.URL,
                name = Emp.name,
                created_at = Emp.created_at.HasValue ? Emp.created_at.Value.ToString("o") : null,
            });
        }


        // Post: api/SalonApi/ReserveCreate
        [HttpPost("ReserveCreate")]
        public string ReserveCreate([FromForm]Reserve reserve)
        {


            _context.Reserves.Add(reserve);
            _context.SaveChangesAsync();

            return "預約完成!";
        }


    }
}
