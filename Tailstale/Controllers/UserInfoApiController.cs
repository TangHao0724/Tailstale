using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tailstale.Models;
using Tailstale.UserInfoDTO;
using System.Reflection.PortableExecutable;

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


        //傳送pet
        //route api/UserInfoApi/GetPet
        [HttpGet("GetPet")]
        public async Task<IActionResult> GetPet([FromQuery] ApiInputID input)
        {
            //抓使用者所有的寵物資訊
            //得到每一筆的寵物資訊的寵物名稱

            //抓寵物資料
            var pets = await _context.pets.Where(n => n.keeper_ID == input.ID)
                                          .Select( pt => new 
                                          {
                                              ID = pt.pet_ID,
                                              name = pt.name,
                                              chip_ID = pt.chip_ID,
                                              age = pt.age,
                                              birthday = pt.birthday,
                                              gender = pt.gender,
                                              pet_type_ID = pt.pet_type_ID,
                                              pet_weight = pt.pet_weight,
                                              vaccine = pt.vaccine,
                                              neutered = pt.neutered,
                                              allergy = pt.allergy,
                                              chronic_dis = pt.chronic_dis,
                                              memo = pt.memo,
                                              created_at = pt.created_at
                                          })
                                          .ToListAsync();
            var keeperImgTypes = await _context.keeper_img_types.ToListAsync();
            var keeperImgs = await _context.keeper_imgs.ToListAsync();

            var result = pets.Select(pet =>
            {
                int? matchingType = keeperImgTypes.Where(kit => kit.FK_Keeper_id== input.ID && kit.typename == "pet_"+pet.ID+"_"+pet.name+"_head")?.Select(s=>s.ID).FirstOrDefault();
                var imgurl = keeperImgs.Where(n => n.img_type_id == matchingType && n.name.Contains("head") )
                                        .OrderByDescending(x => x.created_at)
                                        .Select(s => s.URL)
                                        .FirstOrDefault();
                return new
                {
                    pet.ID,
                    pet.chip_ID,
                    pet.name,
                    pet.age,
                    pet.birthday,
                    pet.gender,
                    pet.pet_type_ID,
                    pet.pet_weight,
                    pet.vaccine,
                    pet.neutered,
                    pet.allergy,
                    pet.chronic_dis,
                    pet.memo,
                    pet.created_at,
                    imgurl = imgurl ?? "no_image.png"
                };
            }).ToList();

            //抓寵物圖片，如果沒有抓到，則顯示NOIMG以，


            return new JsonResult(result);
        }


        //傳送指定用戶的pet
        //route api/UserInfoApi/petTypes
        [HttpGet("GetPetTypes")]
        public async Task<IActionResult> GetPetTypes()
        {
            var petTypes = await _context.pet_types
                .Select(pt => new GetPetTypeDTO
                {
                    ID = pt.ID,
                    species = pt.species,
                    breed = pt.breed
                })
                .ToListAsync();
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

        //修改會員資料資料
        [HttpPut("updateKeeper")]
        public async Task<IActionResult> PutKeeperInfo([FromBody] UserDetailDTO DTO)
        {
            var keeper = await _context.keepers.FindAsync(DTO.ID);
            if (keeper == null)
            {
                return NotFound();
            }

            keeper.name = DTO.name;
            keeper.address = DTO.address;
            keeper.email = DTO.email;
            keeper.phone = DTO.phone;
            _context.Entry(keeper).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!keeperExists(DTO.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(new { Message = "更新成功！" });
        }

        //修改寵物資料
        [HttpPut("updatePetInfo")]
        public async Task<IActionResult> updatePetInfo([FromBody] PetInfoDTO DTO)
        {
            var pet = await _context.pets.FindAsync(DTO.ID);
            if (pet == null)
            {
                return NotFound();
            }

            pet.name = DTO.name;
            pet.pet_type_ID = DTO.pet_type_ID;
            pet.chip_ID = DTO.chip_ID;
            pet.gender = DTO.gender;
            pet.birthday  =DTO.birthday;
            pet.age = DTO.age;
            pet.pet_weight = DTO.pet_weight;
            pet.vaccine = DTO.vaccine;
            pet.neutered = DTO.neutered;
            pet.allergy = DTO.allergy;
            pet.chronic_dis = DTO.chronic_dis;
            pet.memo = DTO.memo;
            _context.Entry(pet).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!keeperExists(DTO.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(new { Message = "更新成功！" });
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
