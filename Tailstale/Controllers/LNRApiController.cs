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
using Microsoft.CodeAnalysis.Scripting;

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
        //Keeper會員註冊
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
        //Business會員註冊
        [HttpPost("BRegister")]
        public async Task<IActionResult> BRegister([FromForm] BRegisterDTO DTO)
        {
            //檢查帳號資訊是否重複

            if (await _context.businesses.AnyAsync(m => m.email == DTO.email))
            {
                return Ok(new { Message = $"有重複帳號，請重新提交" });
            }

            //生成鹽
            var KSalt = CreateSalt();
            //將密碼加密
            byte[] HashedPsw = HashingPsw(DTO.password, KSalt);

            //存入DTO獲得帳號資訊
            _context.Add(new business
            {
                name = DTO.name,
                email = DTO.email,
                password = Convert.ToBase64String(HashedPsw),
                salt = Convert.ToBase64String(KSalt),
                business_status = 1,
                type_ID = DTO.bType 
            });

            await _context.SaveChangesAsync();

            return Ok(new { Message = $"完成帳號；{DTO.name}的建立。" });
        }
        //生成鹽
        /*
        * 將狀態存入session
        1. 是否登入(驗證有沒有session)
        2. 目前會員編號(ID)
        3. 目前身分。(int表示)
        存入(1.ID 2.身分)
        身分：
        0. Keeper 
        1. User
        2. Salon
        3. Hospital
        99.admin
        */
        //Keeper會員登入
        [HttpPost("KLogin")]
        public async Task<IActionResult> KLogin([FromForm] LoginDTO DTO)
        {
            //管理者登入
            if(DTO.email == "admin@admin" && DTO.password == "admin")
            {
                HttpContext.Session.SetInt32("loginID",99999);//登入成功，建立session
                HttpContext.Session.SetInt32("loginType", 99);
                return Ok(new { Message = $"登入成功，admin" });
                //回傳OK

            }
            //確認真的有該帳號
            if (!await _context.keepers.AnyAsync(m => m.email == DTO.email))
            {
                return Ok(new { Message = $"帳號有誤，請重新提交" });
            }

            //將密碼加密
            var hashSalt  = await  _context.keepers.Where(m=> m.email == DTO.email).Select(s => s.salt).FirstOrDefaultAsync();

            var hashedPsw = HashingPsw(DTO.password, Convert.FromBase64String(hashSalt));

            string stringHashedPsw = Convert.ToBase64String(hashedPsw);
            //將資料庫的密碼轉為陣列
            var pswinDB = await _context.keepers.Where(m => m.email == DTO.email).Select(s => s.password).FirstOrDefaultAsync();
            //比對加密密碼是否與DB密碼相符
            
            if (stringHashedPsw != pswinDB)
            {
                return Ok(new { Message = $"密碼有誤，請重新提交" });
            }
            int selectID = await _context.keepers.Where(m => m.email == DTO.email).Select(s => s.ID).FirstOrDefaultAsync();


            HttpContext.Session.SetInt32("loginID", selectID);//登入成功，建立session
            HttpContext.Session.SetInt32("loginType", 0);


            //return RedirectToAction("Privacy", "Home");
            return Ok(new { Message = $"登入成功，用戶：{selectID}", LoginID = HttpContext.Session.GetInt32("loginID"), LoginType = HttpContext.Session.GetInt32("loginType") });
            //回傳OK

        }
        //business登入
        [HttpPost("BLogin")]
        public async Task<IActionResult> BLogin([FromForm] LoginDTO DTO)
        {
            //管理者登入
            if (DTO.email == "admin@admin" && DTO.password == "admin")
            {
                HttpContext.Session.SetInt32("loginID", 99999);//登入成功，建立session
                HttpContext.Session.SetInt32("loginType", 99);
                return Ok(new { Message = $"登入成功，admin" });
                //回傳OK

            }
            //確認真的有該帳號
            if (!await _context.businesses.AnyAsync(m => m.email == DTO.email))
            {
                return Ok(new { Message = $"帳號有誤，請重新提交" });
            }

            //將密碼加密
            var hashSalt = await _context.businesses.Where(m => m.email == DTO.email).Select(s => s.salt).FirstOrDefaultAsync();

            byte[] salt = Convert.FromBase64String(hashSalt);

            var hashedPsw = HashingPsw(DTO.password, salt);
            var stringhashedPsw = Convert.ToBase64String(hashedPsw);
            //將資料庫的密碼轉為陣列
            var pswinDB = await _context.businesses.Where(m => m.email == DTO.email).Select(s => s.password).FirstOrDefaultAsync();
            //比對加密密碼是否與DB密碼相符

            if (stringhashedPsw != pswinDB)
            {
                return Ok(new { Message = $"密碼有誤，請重新提交" });
            }
            int selectID = await _context.businesses.Where(m => m.email == DTO.email).Select(s => s.ID).FirstOrDefaultAsync();

            int? bType = await _context.businesses.Where(m => m.email == DTO.email).Select(m => m.type_ID).FirstOrDefaultAsync();

            HttpContext.Session.SetInt32("loginID", selectID);//登入成功，建立session
            HttpContext.Session.SetInt32("loginType", (Int32)bType);

            return Ok(new { Message = $"登入成功，用戶：{selectID}", LoginID = HttpContext.Session.GetInt32("loginID"), LoginType = HttpContext.Session.GetInt32("loginType") });
            //回傳OK

        }
        //登出
        [HttpDelete("Logout")]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            return Ok(new { Message = $"完成登出" });
        }
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
