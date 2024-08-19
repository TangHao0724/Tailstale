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
using MailKit.Search;
using AutoMapper.Internal;
using Org.BouncyCastle.Asn1.X509;

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
                                          .Select(pt => new
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
                int? matchingType = keeperImgTypes.Where(kit => kit.FK_Keeper_id == input.ID && kit.typename == "pet_" + pet.ID + "_" + pet.name + "_head")?.Select(s => s.ID).FirstOrDefault();
                var imgurl = keeperImgs.Where(n => n.img_type_id == matchingType && n.name.Contains("head"))
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
                return Ok(new { message = $"新增成功 pet_ID = {pet.pet_ID}" });
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
            pet.birthday = DTO.birthday;
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
        //最近一周內貼文
        [HttpGet("getcount")]
        public async Task<IActionResult> GetCount([FromQuery] int id)
        {
            var article = await _context.articles.Where(a => a.FK_Keeper_ID == id).ToListAsync();
            var weektime = DateTime.Now.AddDays(-7);
            //抓到一周以內的時間
            var count = article.Where(n => n.created_at >= weektime).Count();
            return Ok(count);
        }
        //最新貼文
        [HttpGet("getLatestPosts")]
        public async Task<IActionResult> GetLatestPosts([FromQuery] int id)
        {
            var articles = await _context.articles.ToListAsync();
            if (articles == null || !articles.Any())
            {
                return Ok((object)null);
            }

            int idart = articles.Count(n => n.FK_Keeper_ID == id);
            int takeCount = idart < 3 ? idart : 3;

            var newpost = articles.Where(n => n.FK_Keeper_ID == id)
                                  .OrderByDescending(x => x.created_at)
                                  .Take(takeCount)
                                  .Select(s => new
                                  {
                                      s.ID,
                                      s.FK_Keeper_ID,
                                      s.parent_ID,
                                      s.content,
                                      s.article_imgs,
                                      s.created_at
                                  })
                                  .ToList();
            if (newpost == null || !newpost.Any())
            {
                newpost = null;
            }

            return Ok(newpost);
        }
        //最新得到的回覆
        [HttpGet("getArticleReplies")]
        public async Task<IActionResult> GetArticleReplies([FromQuery] int id)
        {
            var articles = await _context.articles.ToListAsync();
            if (articles == null || !articles.Any())
            {
                return Ok((object)null);
            }

            var allUserArt = articles.Where(n => n.FK_Keeper_ID == id).Select(s => s.ID).ToList();
            var allresp = articles.Count(n => n.parent_ID.HasValue && allUserArt.Contains(n.parent_ID.Value));
            int allrespCount = allresp < 3 ? allresp : 3;

            var newallrespResult = articles.Where(n => n.parent_ID.HasValue && allUserArt.Contains(n.parent_ID.Value))
                                           .OrderByDescending(x => x.created_at)
                                           .Take(allrespCount)
                                            .Select(s => new
                                            {
                                                s.ID,
                                                s.FK_Keeper_ID,
                                                s.parent_ID,
                                                s.content,
                                                s.article_imgs,
                                                s.created_at
                                            })
                                           .ToList();
            if (newallrespResult == null || !newallrespResult.Any())
            {
                newallrespResult = null;
            }

            return Ok(newallrespResult);
        }
        //新增一周內預約
        [HttpGet("getNewAppointments")]
        public async Task<IActionResult> GetNewAppointmentsAndPets([FromQuery] int id)
        {
            var weektime = DateTime.Now.AddDays(-7);
            // 新增的預約
            List<AppointmentResult> newallorderRe = new List<AppointmentResult>();
            var allorder = await UsertOrder(id);
            if (allorder is OkObjectResult combinedResult)
            {
                newallorderRe = combinedResult.Value as List<AppointmentResult>;
            }
            if (newallorderRe == null || !newallorderRe.Any())
            {
                newallorderRe = null;
            }
            else
            {
                int ordercount = newallorderRe.Count < 3 ? newallorderRe.Count : 3;
                newallorderRe = newallorderRe
                    .Where(x=> DateTime.Parse(x.OrderDate) > weektime)
                    .OrderByDescending(x => x.OrderDate)
                                             .Take(ordercount)
                                             .ToList();
            }
            return Ok(newallorderRe);
        }
        [HttpGet("newPictures")]
        public async Task<IActionResult> GetNewPictures([FromQuery] int id)
        {
            try
            {
                // 查詢所有相片類型
                var imgTypeIds = await _context.keeper_img_types
                    .Where(n => n.FK_Keeper_id == id)
                    .Select(s => s.ID)
                    .ToListAsync();

                // 查詢最新的相片URL
                var latestImage = await _context.keeper_imgs
                    .Where(n => imgTypeIds.Contains((int)n.img_type_id))
                    .OrderByDescending(n => n.created_at)
                    .Select(s => new
                    {
                        s.URL,
                        s.img_type.typename,
                        s.created_at,
                    })
                    .FirstOrDefaultAsync();

                return Ok(latestImage ?? null);
            }
            catch (Exception ex)
            {
                // 處理異常
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetNewpets")]

        public async Task<IActionResult> GetNewPets([FromQuery] int id)
        {
            var pets = await _context.pets.Where(n => n.keeper_ID == id).ToListAsync();
            List<GetMainPetDTO> petDTOs = new List<GetMainPetDTO>();
            if (pets == null || !pets.Any())
            {
                petDTOs = null;
            }
            else
            {
                int petsCount = pets.Count < 3 ? pets.Count : 3;
                petDTOs = pets.OrderByDescending(x => x.created_at)
                              .Take(petsCount)
                              .Select(s => new GetMainPetDTO
                              {
                                  pets_type = $"{s.pet_type.species},{s.pet_type.breed}",
                                  name = s.name,
                                  age = s.age,
                                  gender = s.gender,
                                  birthday = s.birthday,
                              })
                              .ToList();
            }
            return Ok(petDTOs);
        }
            [HttpGet("UserOrder/{id}")]
        public async Task<IActionResult> UsertOrder(int id)
        {
            var bookings = await _context.Bookings.ToListAsync();
            var reserves = await _context.Reserves.ToListAsync();
            var appointments = await _context.Appointments.ToListAsync();
            var pets = await _context.pets.ToListAsync();
            var businesses = await _context.businesses.ToListAsync();
            var status = await _context.order_statuses.ToListAsync();


            var bookingResult = bookings.Where(n => n.keeper_ID == id).Select(s => new
            {
                orderID = s.bookingID,
                orderPet = pets.FirstOrDefault(p => p.pet_ID == s.CheckinDetails.FirstOrDefault(cd => cd.bookingID == s.bookingID).pet_ID)?.name,
                orderType = "寵物旅館",
                businessName = s.hotel.name,
                serviceName = "旅館住宿",
                orderDate = s.bookingDate,
                orderStatus = status.FirstOrDefault(n=>n.ID == s.bookingStatus).status_name,
            }).ToList();

            var reserveResult = reserves.Where(n => n.keeper_id == id).Select(s => new
            {
                orderID = s.id,
                orderPet = s.pet_name,
                orderType = "寵物美容",
                businessName = s.business.name,
                serviceName = s.service_name,
                orderDate = s.created_at,
                orderStatus = status.FirstOrDefault(n => n.ID == s.status).status_name,
            }).ToList();

            var appointmentResult = appointments.Where(n => n.keeper_ID == id).Select(s => new
            {
                orderID = s.Appointment_ID,
                orderPet = s.pet.name,
                orderType = "寵物醫療",
                businessName = businesses.FirstOrDefault(b => b.ID == s.daily_outpatient_clinic_schedule.outpatient_clinic.vet.business_ID)?.name,
                serviceName = $"看診：{s.daily_outpatient_clinic_schedule.outpatient_clinic.outpatient_clinic_name}",
                orderDate = s.registration_time,
                orderStatus = status.FirstOrDefault(n => n.ID == s.Appointment_status).status_name,
            }).ToList();

            var combinedResult = bookingResult.Concat(reserveResult).Concat(appointmentResult).ToList();

            return Ok(combinedResult);
        }

        //傳遞main的所有資料   
        //route api/UserInfoApi/PostPetInfo
        //[HttpGet("getmain")]
        //public async Task<IActionResult> getmain([FromQuery] int id)
        //{
        //    List<article> newpost = new List<article>();
        //    var articles = await _context.articles.ToListAsync();
        //    if (articles == null || !articles.Any())
        //    {
        //        newpost = null;
        //    }

        //    int idart = articles.Count(n => n.FK_Keeper_ID == id);
        //    int takeCount = idart < 3 ? idart : 3;

        //    // 最新貼文
        //     newpost = articles.Where(n => n.FK_Keeper_ID == id)
        //                          .OrderByDescending(x => x.created_at)
        //                          .Take(takeCount)
        //                          .ToList();
        //    if (newpost == null || !newpost.Any())
        //    {
        //        newpost = null;
        //    }

        //    // 文章的回覆
        //    var allUserArt = articles.Where(n => n.FK_Keeper_ID == id).Select(s => s.ID).ToList();
        //    var allresp = articles.Count(n => allUserArt.Contains((int)n.parent_ID));
        //    int allrespCount = allresp >0 ? allresp < 3 ? allresp : 3 : 0;

        //    var newallrespResult = articles.Where(n => allUserArt.Contains((int)n.parent_ID))
        //                                   .OrderByDescending(x => x.created_at)
        //                                   .Take(allrespCount)
        //                                   .ToList();
        //    if (newallrespResult == null || !newallrespResult.Any())
        //    {
        //        newallrespResult = null;
        //    }

        //    // 新增的預約
        //    List<AppointmentResult> newallorderRe = new List<AppointmentResult>();
        //    var allorder = await UsertOrder(id);
        //    if (allorder is OkObjectResult combinedResult)
        //    {
        //        newallorderRe = combinedResult.Value as List<AppointmentResult>;
        //    }
        //    if (newallorderRe == null || !newallorderRe.Any())
        //    {
        //        newallorderRe = null;
        //    }
        //    else
        //    {
        //        int ordercount = newallorderRe.Count < 3 ? newallorderRe.Count : 3;
        //        newallorderRe = newallorderRe.OrderByDescending(x => x.OrderDate)
        //                                     .Take(ordercount)
        //                                     .ToList();
        //    }

        //    // 新增的寵物
        //    var pets = await _context.pets.Where(n => n.keeper_ID == id).ToListAsync();
        //    List< GetMainPetDTO > petDTOs = new List< GetMainPetDTO >();
        //    if (pets == null || !pets.Any())
        //    {
        //        petDTOs = null;
        //    }
        //    else
        //    {
        //        int petsCount = pets.Count < 3 ? pets.Count : 3;
        //        petDTOs = pets.OrderByDescending(x => x.created_at)
        //                          .Take(petsCount)
        //                          .Select(s => new GetMainPetDTO
        //                          {
        //                              pets_type = $"{s.pet_type.specimen},{s.pet_type.breed}",
        //                              name = s.name,
        //                              age = s.age,
        //                              gender = s.gender,
        //                              birthday = s.birthday,
        //                          })
        //                          .ToList();

        //    }

        //    var end = new
        //    {
        //        newpost,
        //        newallrespResult,
        //        newallorderRe,
        //        newpet = petDTOs,
        //    };
        //    return Ok(end);
        //}
        
        public class AppointmentResult
        {
            public int OrderID { get; set; }
            public string OrderPet { get; set; }
            public string OrderType { get; set; }
            public string BusinessName { get; set; }
            public string ServiceName { get; set; }
            public string OrderDate { get; set; }
            public string OrderStatus { get; set; }
        }
        private bool keeperExists(int id)
        {
            return _context.keepers.Any(e => e.ID == id);
        }
    }
}
