using HotelAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelAPI.Controllers
{
    public class HotelsController : Controller
    {

        public TailstaleContext _context;

        public HotelsController(TailstaleContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> FindPet(int id)
        {
            var a = _context.pets.Where(p => p.pet_ID == id).Select(k => new CheckInDTO
            {
                petID = k.pet_ID,
                petName = k.name,
                petChipID = k.chip_ID,
                petBirthDay = k.birthday.Value == null ? null : k.birthday.Value,
                petType = k.pet_type.species

            }).FirstOrDefault();

            if (a == null)
            {
                return NotFound(); // 返回 404 Not Found
            }

            return Ok(a);


        }
    }
}
