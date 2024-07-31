using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tailstale.Models;
using Tailstale.KImgDTO;
using System.Diagnostics.Metrics;

namespace Tailstale.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KImgController : ControllerBase
    {
        private readonly TailstaleContext _context;

        public KImgController(TailstaleContext context)
        {
            _context = context;
        }
        //單次匯入圖片
        [HttpPost("UploadsingleImg")]
        public async Task<IActionResult> UploadsingleImg(singleImgDTO DTO)
        {
            try
            {
                if (DTO == null || DTO.img == null)
                {
                    return BadRequest("Invalid input data.");
                }
                if (DTO.img.Length == 0)
                {
                    return BadRequest("Empty file uploaded.");
                }
                if (!_context.keeper_img_types.Where(n => n.FK_Keeper_id == DTO.UserID).Any(x => x.typename.Contains(DTO.type_name)))
                {
                    //2.2.1沒有，先在TYPE建立輸入的typeNAME
                    var newKeeperImgType = new keeper_img_type
                    {
                        FK_Keeper_id = DTO.UserID,
                        typename = DTO.type_name
                    };

                    _context.Add(newKeeperImgType);
                    await _context.SaveChangesAsync();

                }
                //2.2.2將相片存入該TYPE
                //2.2.2.1 使用隨機數改掉URL名稱
                //2.2.2.2 將新的名稱存入keeper_img
                //2.2.2.3 使用新的名稱將檔案複製入喜統
                var newURL = GenerateRandomString(10) + Path.GetExtension(DTO.img.FileName);

                int typeID = await _context.keeper_img_types
                                .Where(n => n.typename == DTO.type_name && n.FK_Keeper_id == DTO.UserID)
                                .Select(n => n.ID)
                                .FirstOrDefaultAsync();
                string Name = DTO.img_name;
                int counter = 1;
                while (_context.keeper_imgs.Where(t => t.img_type_id == typeID).Any(n => n.name == DTO.img_name))
                {
                    Name = $"{DTO.img_name}_{counter}";
                    counter++;
                }

                var newKeeperImg = new keeper_img
                {
                    img_type_id = typeID,
                    URL = newURL,
                    name = Name,
                };
                _context.Add(newKeeperImg);
                await _context.SaveChangesAsync();
                //2.2.3 存入圖片到本地指定
                //資料夾
                //建立資料夾
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imgs/keeper_img", DTO.type_name);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var filePath = Path.Combine(folderPath, newURL);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await DTO.img.CopyToAsync(stream);
                }

                return Ok(new { Message = $"已在{DTO.type_name}存入相片：{DTO.img_name}檔名：{newURL}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"上傳圖片失敗: {ex.Message}" });
            }

        }

        //讀取指定相簿的指定圖片，得到URL
        [HttpGet("GetsingleImg")]
        public Task<IActionResult> GetSingleImg()
        {
            return 
        }
        //讀取一個相簿的所有圖片
        //更換相片的類型
        //讀取最新的一張圖片
        //r

        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
