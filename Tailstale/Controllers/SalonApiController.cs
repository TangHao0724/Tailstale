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

        // GET: api/SalonApi
        [HttpGet]
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

    }
}
