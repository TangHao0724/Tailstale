using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Tailstale.Index_DTO;
using Tailstale.Models;

namespace Tailstale.Controllers
{

    [Route("/api/ImgApi")]
    [ApiController]
    public class ImgApiController : Controller
    {
        private readonly TailstaleContext _context;

        public ImgApiController(TailstaleContext context)
        {
            _context = context;
        }
        //建立相簿並存入圖片
        [RequestFormLimits(MultipartBodyLengthLimit = 2048000)]
        [RequestSizeLimit(2048000)]
        [HttpPost("UploadsingleImg")]
        public async Task<IActionResult> UploadsingleImg(singleImgDTO DTO)
        {
            try {
                if (!_context.keeper_img_types.Any(x => x.typename.Contains(DTO.type_name)))
                {
                    //2.2.1沒有，先在TYPE建立輸入的typeNAME
                    var newKeeperImgType = new keeper_img_type
                    {
                        FK_Keeper_id = DTO.User_id,
                        typename = DTO.type_name
                    };

                    _context.Add(newKeeperImgType);
                    _context.SaveChangesAsync();

                }
                //2.2.2將相片存入該TYPE
                var newKeeperImg = new keeper_img
                {
                    URL = DTO.img.FileName,
                    name = DTO.type_name
                };

                _context.Add(newKeeperImg);
                _context.SaveChangesAsync();
                //2.2.3 存入圖片到本地資料夾
                var path = Path.Combine(
                            Directory.GetCurrentDirectory(), "wwwroot/lib/Index_img",
                             DTO.img.FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await DTO.img.CopyToAsync(stream);
                }
                return Ok(new { Message = $"已在{DTO.type_name}存入相片：{DTO.Imgname}檔名：{DTO.img.FileName}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"上傳圖片失敗: {ex.Message}" });
            }

        }
        //加入ImgType
        [HttpPost("InsertImgType")]
        public async Task<IActionResult> InsertImgType(ImgTypeDTO DTO)
        {
            try
            {

                //2.2.1沒有TYPE，先在TYPE建立輸入的typeNAME
                var newKeeperImgType = new keeper_img_type
                {
                    FK_Keeper_id = DTO.User_id,
                    typename = DTO.type_name
                };

                _context.Add(newKeeperImgType);
                _context.SaveChangesAsync();
                return Ok(new { Message = $"已建立相簿！名稱：{DTO.type_name}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"加入類型失敗: {ex.Message}" });
            }

        }

        //查詢ImgType
        [HttpGet("userImgType")]
        public async Task<IActionResult> userImgType([FromQuery] ApiInputID input)
        {
            if (input.ID < 1000)
            {
                return Ok(new { Message = "沒有此人" });
            }

            //查詢ID的IMGTYPE
            var imgType = await _context.keeper_img_types
                .Where(n => n.FK_Keeper_id == input.ID)
                .ToListAsync();


            if (imgType == null)
            {
                return Ok(new { Message = "尚未建立相簿" });
            }
            return Json(imgType);
        }
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

    }
}
