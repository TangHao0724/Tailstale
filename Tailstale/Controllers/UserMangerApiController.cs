using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tailstale.Index_DTO;
using Tailstale.Models;

namespace Tailstale.Controllers
{
    public class UserMangerApiController : Controller
    {
        private readonly TailstaleContext _context;
        public  UserMangerApiController(TailstaleContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("/api/ UserMangerApi/userInfo")]
        public async Task<JsonResult> userInfo()
        {
            var users = await _context.keepers
                        .Select(u => new
                        {
                            Id = u.ID,
                            Email = u.email,
                            Name = u.name
                            // 添加其他需要返回的列
                        })
                        .ToListAsync();
            return Json(users);
        }

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
