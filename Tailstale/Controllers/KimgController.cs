using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tailstale.Models;
using Tailstale.KImgDTO;
using System.Diagnostics.Metrics;
using System.Security.Policy;
using System.Linq;

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
                    //2.2.1沒有，先在TYPE建立輸入的typeNAME  12_MIMI_
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
                while (_context.keeper_imgs.Where(t => t.img_type_id == typeID).Any(n => n.name == Name))
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
                var path = Path.Combine(
                     Directory.GetCurrentDirectory(), "wwwroot/imgs/keeper_img", newURL);

                using (var stream = new FileStream(path, FileMode.Create))
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
        //匯入複數圖片
        [HttpPost("UploadmuitiImg")]
        public async Task<IActionResult> UploadmuitiImg(muitiImgDTO DTO)
        {
            try
            {
                if (DTO == null || DTO.imgs == null)
                {
                    return BadRequest("Invalid input data.");
                }
                if (!_context.keeper_img_types.Where(n => n.FK_Keeper_id == DTO.UserID).Any(x => x.typename.Contains(DTO.type_name)))
                {
                    //2.2.1沒有，先在TYPE建立輸入的type_name
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

                int typeID = await _context.keeper_img_types
                                .Where(n => n.typename == DTO.type_name && n.FK_Keeper_id == DTO.UserID)
                                .Select(n => n.ID)
                                .FirstOrDefaultAsync();
                foreach (var item in DTO.imgs)
                {
                    if (item.Length == 0)
                    {
                        continue;
                    }
                    var newURL = GenerateRandomString(10) + Path.GetExtension(item.FileName);
                    string Name = Path.GetFileNameWithoutExtension(item.FileName);
                    int counter = 1;
                    while (_context.keeper_imgs.Where(t => t.img_type_id == typeID).Any(n => n.name == Name))
                    {
                        Name = $"{Path.GetFileNameWithoutExtension(item.FileName)}_{counter}";
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
                    //2.2.3 存入圖片到本地指定資料夾

                    var path = Path.Combine(
                         Directory.GetCurrentDirectory(), "wwwroot/imgs/keeper_img", newURL);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await item.CopyToAsync(stream);
                    }

                }
                return Ok(new { Message = $"已在{DTO.type_name}存入相片群" });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"上傳圖片失敗: {ex.Message}" });
            }

        }
        //讀取傳入指定的USERID與相簿名稱，得到最新的URL以及圖片名稱
        [HttpPost("GetsingleImg")]
        public async Task<IActionResult> GetSingleImg(GetsingleImgDTO DTO)
        {
            ImgURLDTO fall = new ImgURLDTO
            {
                img_name = "no_image.png",
                img_url = "no_image.png"
            };
            try
            {
                if (DTO == null || string.IsNullOrEmpty(DTO.type_name) || DTO.UserID <= 0)
                {
                    return BadRequest("Invalid input data.");
                }

                int imgtype = await _context.keeper_img_types
                                            .Where(n => n.FK_Keeper_id == DTO.UserID && n.typename == DTO.type_name)
                                            .Select(i => i.ID)
                                            .FirstOrDefaultAsync();

                if (imgtype == 0)
                {
                    return new JsonResult(fall);
                }

                var img = await _context.keeper_imgs
                                        .Where(n => n.img_type_id == imgtype && n.name.Contains(DTO.img_name))
                                        .OrderByDescending(x => x.created_at)
                                        .FirstOrDefaultAsync();

                if (img == null)
                {

                    return new JsonResult(fall);
                }
                ImgURLDTO url = new ImgURLDTO
                {
                    img_name = img.name,
                    img_url = img.URL
                };
                return new JsonResult(url);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error retrieving image: {ex.Message}" });
            }
        }

        [HttpPost("postImgType")]
        public async Task<IActionResult> postImgType(postImgTypeDTO DTO)
        {
            if(DTO == null)
            {
                return BadRequest("傳入錯誤");
            }
            if (await _context.keeper_img_types.AnyAsync(n => n.FK_Keeper_id == DTO.ID && n.typename == DTO.type_name))
            {
                return BadRequest("重複的相簿名稱");
            }
            try
            {
                var newImgType = new keeper_img_type
                {
                    FK_Keeper_id = DTO.ID,
                    typename = DTO.type_name,
                };

                await _context.AddAsync(newImgType);
                await _context.SaveChangesAsync();

                return Ok(new { newImgType.ID, newImgType.typename });
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here if needed
                return StatusCode(500, "Internal server error");
            }
        }
        //讀取一個相簿的所有圖片
        //更換相片的類型

        //讀取所有的照片類型，並且在每個類型的照片上讀取最新的一張照片作為封面。
        [HttpGet("GetImgType")]
        public async Task<IActionResult> GetImgType(int ID)
        {
            if (!await _context.keeper_img_types.AnyAsync(n => n.FK_Keeper_id == ID))
            {
                return BadRequest("該用戶沒有圖片");
            }
            try
            {
                //1. 讀取輸入用戶的ID，所持有的IMGTYPE。
                var userImgType = await _context.keeper_img_types.Where(n => n.FK_Keeper_id == ID)
                                                                .Select(s => new
                                                                {
                                                                    s.ID,
                                                                    s.typename,
                                                                }).ToListAsync();
                //2. 將所有的imgtype，加入第一章img的照片
                var typeIds = userImgType.Select(u => u.ID).ToList();
                var keeperImgs = await _context.keeper_imgs
                                               .Where(img => typeIds.Contains((int)img.img_type_id))
                                               .ToListAsync();
                //3. 傳回typeID 以及封面的url
                var result = userImgType.Select(r =>
                {

                    var imgurl = keeperImgs.Where(n => n.img_type_id == r.ID)
                            .OrderByDescending(x => x.created_at)
                            .Select(s => s.URL)
                            .FirstOrDefault();
                    return new
                    {
                        r.ID,
                        r.typename,
                        imgurl = imgurl ?? "no_image.png"
                    };
                });
                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
            
        }
        //讀取傳入的類型所有的所有照片。
        [HttpGet("GetTypeAllImg")]
        public async Task<IActionResult> GetTypeAllImg(int typeId)
        {
            if(!await _context.keeper_img_types.AnyAsync(n=>n.ID == typeId))
            {
                return BadRequest("Invalid typeId");
            }
            try
            {
                var alltype = await _context.keeper_imgs
                                            .Where(a => a.img_type_id == typeId)
                                            .Select(n => new
                                            {
                                                n.ID,
                                                n.name,
                                                n.URL,
                                                n.created_at,
                                            })
                                            .ToListAsync();

                return new JsonResult(alltype);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here if needed
                return StatusCode(500, "Internal server error");
            }

        }

        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
