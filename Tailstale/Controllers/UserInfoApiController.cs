using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tailstale.Models;
using Tailstale.UserInfoDTO;

namespace Tailstale.Controllers
{
    public class ApiInputID
    {
        public int ID { get; set; }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class UserInfoApiController : ControllerBase
    {
        private readonly TailstaleContext _context;

        public UserInfoApiController(TailstaleContext context)
        {
            _context = context;
        }
        //傳送Index頁面上詳細內容
        //route api/UserInfoApi/userInfoDetail
        [HttpGet("userInfoDetail")]
        public async Task<IActionResult> userInfoDetail([FromQuery] ApiInputID input)
        {

            var keeper = await _context.keepers
            .Include(k => k.statusNavigation)
            .FirstOrDefaultAsync(m => m.ID == input.ID);

            if (keeper == null)
                return NotFound();

            
            UserDetailDTO result = new UserDetailDTO
            {
                ID = keeper.ID,
                name = keeper.name,
                address = keeper.address,
                email = keeper.email,
                phone = keeper.phone,
                status = keeper.status,
                created_at = keeper.created_at

            };
            return new JsonResult(result);
        }


        //傳送petTypes
        //route api/UserInfoApi/petTypes
        [HttpGet("GetPetTypes")]
        public async Task<IActionResult> GetPetTypes()
        {
            var petTypes = await _context.pet_types.ToListAsync();
            return new JsonResult(petTypes);
        }

        //存入初部資料PostPetInfo
        //route api/UserInfoApi/PostPetInfo
        [HttpPost("PostPetInfo")]
        public async Task<IActionResult> PostPetInfo([FromBody] PostPetDTO postPetDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                pet pet = new pet
                {
                    pet_type_ID = postPetDTO.pet_type_ID,
                    keeper_ID = postPetDTO.keeper_ID,
                    name = postPetDTO.name,
                    gender = postPetDTO.gender,
                    birthday = postPetDTO.birthday,
                    age = postPetDTO.age,

                };
                _context.pets.Add(pet);
                await _context.SaveChangesAsync();
                return Ok(new { message = $"新增成功 pet_ID = {pet.pet_ID}"  });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "內部錯誤", details = ex.Message });
            }
        }
        // GET: api/UserInfoApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<keeper>>> Getkeepers()
        {
            return await _context.keepers.ToListAsync();
        }

        // GET: api/UserInfoApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<keeper>> Getkeeper(int id)
        {
            var keeper = await _context.keepers.FindAsync(id);

            if (keeper == null)
            {
                return NotFound();
            }

            return keeper;
        }

        // PUT: api/UserInfoApi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Putkeeper(int id, keeper keeper)
        {
            if (id != keeper.ID)
            {
                return BadRequest();
            }

            _context.Entry(keeper).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!keeperExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/UserInfoApi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<keeper>> Postkeeper(keeper keeper)
        {
            _context.keepers.Add(keeper);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Getkeeper", new { id = keeper.ID }, keeper);
        }

        // DELETE: api/UserInfoApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletekeeper(int id)
        {
            var keeper = await _context.keepers.FindAsync(id);
            if (keeper == null)
            {
                return NotFound();
            }

            _context.keepers.Remove(keeper);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool keeperExists(int id)
        {
            return _context.keepers.Any(e => e.ID == id);
        }
    }
}
