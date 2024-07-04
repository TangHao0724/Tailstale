using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Tailstale.Index_DTO;
using Tailstale.Models;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components;

namespace Tailstale.Controllers
{
    public class ApiInputID
    {
        public int ID { get; set; }
    }

    [Microsoft.AspNetCore.Mvc.Route("/api/UserMangerApi")]
    [ApiController]
    public class UserMangerApiController : Controller
    {
        //建構函式，不要動
        private readonly TailstaleContext _context;
        private readonly IViewComponentHelper _viewComponentHelper;

        public UserMangerApiController(IViewComponentHelper viewComponentHelper, TailstaleContext context)
        {
            _context = context;
            _viewComponentHelper = viewComponentHelper;
        }


        //傳入Api的ID類型



        //傳送Index頁面上Keeper的ID 跟NAME
        [HttpGet("userInfo")]
        public async Task<JsonResult> userInfo()
        {
            var users = await _context.keepers
                        .Select(u => new
                        {
                            Id = u.ID,
                            Name = u.name
                        })
                        .ToListAsync();
            return Json(users);
        }

        //根據傳入ID 傳送Info頁面
        [HttpGet("userInfoPage")]
        public async Task<IActionResult> userInfoPage([FromQuery] ApiInputID input)
        {
            return PartialView("_Info", input.ID); ;
        }

        //根據傳入ID 傳送Image頁面
        [HttpGet("userImgPage")]
        public async Task<IActionResult> userImgPage([FromQuery] ApiInputID input)
        {
            return PartialView("_Img", input.ID); ;
        }

        //傳送Index頁面上詳細內容
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
                ID = input.ID,
                password = keeper.password,
                name = keeper.name,
                address = keeper.address,
                email = keeper.email,
                phone = keeper.phone,
                status = keeper.status,
                created_at = keeper.created_at



            };
            return Json(result);
        }




        //新增會員
        [HttpPost("PostUser")]
        public async Task<IActionResult> PostUser([FromBody] UserDTO userDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                keeper user = new keeper
                {
                    name = userDTO.name,
                    phone = userDTO.phone,
                    email = userDTO.email,
                    address = userDTO.address,
                    password = userDTO.password,
                    status = 1,

                };
                _context.keepers.Add(user);
                await _context.SaveChangesAsync();
                return Ok(new { message = "用户创建成功", userId = user.ID });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "服务器内部错误", details = ex.Message });
            }
        }
        //刪除USER
        // POST: DeleteUser
        [HttpPost("DeleteUser")]
        public async Task<IActionResult> DeleteUser([FromBody] ApiInputID input)
        {
            if (input == null || input.ID <= 0)
            {
                return BadRequest("輸入錯誤");
            }
            var keeper = await _context.keepers.FindAsync(input.ID);
            if (keeper != null)
            {
                _context.keepers.Remove(keeper);
            }

            await _context.SaveChangesAsync();
            return Ok($"已成功刪除 編號：{input.ID}");
        }
        //更新使用者資訊
        [HttpPost("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UserDetailDTO userDetailDTO )
        {
            var updateTarget = await _context.keepers.FindAsync(userDetailDTO.ID);
                if(updateTarget == null)
            {
                return NotFound(new { message = $"User with ID {userDetailDTO.ID} not found" });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {

                updateTarget.name = userDetailDTO.name;
                updateTarget.email = userDetailDTO.email;
                updateTarget.phone = userDetailDTO.phone;
                updateTarget.password = userDetailDTO.password;
                updateTarget.address = userDetailDTO.address;
                updateTarget.status = userDetailDTO.status;
                
                _context.keepers.Update(updateTarget);
                await _context.SaveChangesAsync();
                return Ok(new { message = "更新成功", userId = updateTarget.ID });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "內部錯誤", details = ex.Message });
            }
        }

    }
}


