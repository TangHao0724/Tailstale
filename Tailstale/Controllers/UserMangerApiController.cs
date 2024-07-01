using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Tailstale.Index_DTO;
using Tailstale.Models;
using Tailstale.Index_ViewCpmpoment;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;

namespace Tailstale.Controllers
{
    public class UserMangerApiController : Controller
    {


        private readonly TailstaleContext _context;
        public  UserMangerApiController(TailstaleContext context)
        {
            _context = context;
        }


        //傳送Index頁面上的ID 跟NAME
        [HttpGet]
        [Route("/api/ UserMangerApi/userInfo")]
        public async Task<JsonResult> userInfo()
        {
            var users = await _context.keepers
                        .Select(u => new
                        {
                            Id = u.ID,
                            Name = u.name
                            // 添加其他需要返回的列
                        })
                        .ToListAsync();
            return Json(users);
        }

        //根據傳入ID 傳送userInfoView頁面上 該位Keeper的所有資料
        [HttpGet]
        [Route("/api/UserMangerApi/userInfodetail")]
        public async Task<IActionResult> userInfodetail([FromQuery] int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var keeper = await _context.keepers.FindAsync(id);
            if (keeper == null)
            {
                return NotFound();
            }
            return Json(keeper);
        }

        //新增會員
        [HttpPost]
        [Route("/api/UserMangerApi/PostUser")]
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
            catch (Exception ex) {
                return StatusCode(500, new { message = "服务器内部错误", details = ex.Message });
            }
        }
       
    }

    
}
