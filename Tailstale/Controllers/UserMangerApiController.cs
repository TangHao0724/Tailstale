using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Tailstale.Index_DTO;
using Tailstale.Models;
using Tailstale.Index_ViewComponent;
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

        public UserMangerApiController( TailstaleContext context)
        {
            _context = context;
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

        //根據傳入ID 傳送userInfoView頁面上 該位Keeper的所有資料
        [HttpGet("userInfoDetail")]
        public async Task<IActionResult> userInfodetail([FromQuery] ApiInputID input)
        {

            var keeper = await _context.keepers
                .Include(k => k.statusNavigation)
                .FirstOrDefaultAsync(m => m.ID == input.ID);

            if (keeper == null)
            {
                return NotFound();
            }
            var UserDetailDTO = new UserDetailDTO
            {
                ID = keeper.ID,
                password = keeper.password,
                name = keeper.name,
                phone = keeper.phone,
                email = keeper.email,
                address = keeper.address,
                status = keeper.status,

            };
            return Json(UserDetailDTO);
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
                return Ok(new { message = "建立成功", userId = user.ID });
            }
            catch (Exception ex) {
                return StatusCode(500, new { message = "服务器内部错误", details = ex.Message });
            }
        }
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
            return Ok($"已成功刪除 編號：{input}") ;
        }

        private bool keeperExists(int id)
        {
            return _context.keepers.Any(e => e.ID == id);
        }

    }


    
}
