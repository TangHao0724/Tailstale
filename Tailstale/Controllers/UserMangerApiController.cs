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
        [HttpGet("userInfodetail")]
        public async Task<IActionResult> userInfodetail([FromQuery] ApiInputID input)
        {
            var html = await GetInfoViewComponentHtml("InfoViewComponent", input.ID);

            return Content(html, "text/html");
        }

        public async Task<string> GetInfoViewComponentHtml(string componentName, int ID)
        {
            var viewComponentResult = await HttpContext.RequestServices.GetRequiredService<IViewComponentHelper>()
               .InvokeAsync(componentName, new { ID });
            using (var writer = new StringWriter())
            {
                viewComponentResult.WriteTo(writer, HtmlEncoder.Default);   
                return writer.ToString();
            }
        }

        [HttpGet("uu")]
        public async Task<string> uu([FromQuery] ApiInputID input)
        {
            var viewComponentResult = await HttpContext.RequestServices.GetRequiredService<IViewComponentHelper>()
            .InvokeAsync("InfoComponent", new { input.ID });
            using (var writer = new StringWriter())
            {
                viewComponentResult.WriteTo(writer, HtmlEncoder.Default);
                var showhtml = writer.ToString();
                return showhtml;
            }
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
            catch (Exception ex) {
                return StatusCode(500, new { message = "服务器内部错误", details = ex.Message });
            }
        }
       
    }


    
}
