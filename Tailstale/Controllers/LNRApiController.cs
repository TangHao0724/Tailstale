using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Konscious.Security.Cryptography;
using Tailstale.Models;
using System.Security.Cryptography;
using System.Text;
using Tailstale.LNR_DTO;
using Microsoft.EntityFrameworkCore;

namespace Tailstale.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LNRApiController : ControllerBase
    {
        private readonly TailstaleContext _context;

        public LNRApiController(TailstaleContext context)
        {
            _context = context;
        }

        [HttpPost("KRegister")]
        public async Task<IActionResult> KRegister([FromForm]KRegisterDTO DTO)
        {
            //檢查帳號資訊是否重複

            if (await _context.keepers.AnyAsync(m => m.email == DTO.email))
            {
                return Ok (new { Message = $"有重複帳號，請重新提交" });
            }

            //生成鹽
            var KSalt = CreateSalt();
            //將密碼加密
            byte[] HashedPsw = HashingPsw(DTO.password , KSalt);
            
            //存入DTO獲得帳號資訊
            _context.Add(new keeper
            {
                name = DTO.name,
                email = DTO.email,
                password = Convert.ToBase64String(HashedPsw),
                salt = Convert.ToBase64String(KSalt),
                status = 1
            });

            await _context.SaveChangesAsync();

            return Ok(new { Message = $"完成帳號；{DTO.name}的建立。" });
        }
        //生成鹽
        private byte[] CreateSalt()
        {
            var buffer = new byte[16];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(buffer);
            return buffer;
        }
        //密碼加密
        public byte[] HashingPsw(string psw, byte[] salt)
        {
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(psw));

            //底下這些數字會影響運算時間，而且驗證時要用一樣的值
            argon2.Salt = salt;
            argon2.DegreeOfParallelism = 8; // 4 核心就設成 8
            argon2.Iterations = 4; // 迭代運算次數
            argon2.MemorySize = 1024 * 1024; // 1 GB

            return argon2.GetBytes(16);
        }
    }
}
