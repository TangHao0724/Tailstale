using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using Tailstale.Models;
using Tailstale.UserInfoDTO;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tailstale.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class socialController : ControllerBase
    {
        private readonly TailstaleContext _context;

        public socialController(TailstaleContext context)
        {
            _context = context;
        }
        //寫入keeper文章以及文章圖片關聯表，完持一次的文章貼文
        [HttpPost("PostArticle")]
        public async Task<IActionResult> PostArticle(postArticleDTO DTO)
        {
            if (DTO == null)
            {
                return BadRequest("你的輸入為空");
            }
            try
            {
                // 判斷發文者身分
                var newArticle = new article { };
                string article_typename = "article";

                // 如果有 parentID 判斷是否有該文章
                if (DTO.parent_ID != null)
                {
                    if (_context.articles.Any(n => n.ID == DTO.parent_ID))
                    {
                        newArticle.parent_ID = DTO.parent_ID;
                    }
                    else
                    {
                        return BadRequest("找不到您回復的文章");
                    }
                }

                // 判斷是 Keeper 還是 Business
                if (DTO.Keeper_ID != null && DTO.Business_ID == null)
                {
                    newArticle.FK_Keeper_ID = DTO.Keeper_ID;
                }
                else if (DTO.Business_ID != null && DTO.Keeper_ID == null)
                {
                    newArticle.FK_Business_ID = DTO.Business_ID;
                }
                else
                {
                    return BadRequest("發文者身份不明");
                }

                newArticle.content = DTO.Content;
                newArticle.ispublic = DTO.isPublic;

                _context.Add(newArticle);
                await _context.SaveChangesAsync();

                // 判斷是否有圖片
                if (DTO.imgs == null || DTO.imgs.Count == 0)
                {
                    return Ok(new { Message = $"PO文成功!沒有圖片 ID{newArticle.ID}" });
                }

                // 處理圖片類型
                if (DTO.Keeper_ID != null && DTO.Business_ID == null)
                {
                    if (!_context.keeper_img_types.Where(n => n.FK_Keeper_id == DTO.Keeper_ID).Any(x => x.typename.Equals(article_typename)))
                    {
                        var newKeeperImgType = new keeper_img_type
                        {
                            FK_Keeper_id = DTO.Keeper_ID,
                            typename = article_typename,
                        };
                        _context.Add(newKeeperImgType);
                        await _context.SaveChangesAsync();
                    }
                    var typeid = await _context.keeper_img_types.Where(n => n.FK_Keeper_id == DTO.Keeper_ID && n.typename.Equals(article_typename)).Select(s => s.ID).FirstAsync();
                    await save_Kimg_intype((int)DTO.Keeper_ID, typeid, DTO.imgs);

                    var imgsName = await _context.keeper_imgs.Where(f => f.img_type_id == typeid).Select(s => s.ID).ToListAsync();
                    var articleImgs = imgsName.Select(img => new article_img
                    {
                        FK_article_ID = newArticle.ID,
                        FK_Keeper_img_ID = img
                    }).ToList();

                    _context.article_imgs.AddRange(articleImgs);
                }
                else if (DTO.Business_ID != null && DTO.Keeper_ID == null)
                {
                    if (!_context.business_img_types.Where(n => n.FK_business_id == DTO.Business_ID).Any(x => x.typename.Equals(article_typename)))
                    {
                        var newbusinessImgType = new business_img_type
                        {
                            FK_business_id = DTO.Business_ID,
                            typename = article_typename,
                        };
                        _context.Add(newbusinessImgType);
                        await _context.SaveChangesAsync();
                    }
                    var typeid = await _context.business_img_types.Where(n => n.FK_business_id == DTO.Business_ID && n.typename.Equals(article_typename)).Select(s => s.ID).FirstAsync();
                    await save_Bimg_intype((int)DTO.Business_ID, typeid, DTO.imgs);

                    var imgsName = await _context.business_imgs.Where(f => f.img_type_id == typeid).Select(s => s.ID).ToListAsync();
                    var articleImgs = imgsName.Select(img => new article_img
                    {
                        FK_article_ID = newArticle.ID,
                        FK_Business_img_ID = img
                    }).ToList();

                    _context.article_imgs.AddRange(articleImgs);
                }

                await _context.SaveChangesAsync();

                return Ok(new { Message = $"PO文成功!包含圖片 ID{newArticle.ID}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"上傳圖片失敗: {ex.Message}" });
            }
        }
        private async Task save_Kimg_intype(int userid, int typeid, List<IFormFile> imgs)
        {
            try
            {
                foreach (var item in imgs)
                {
                    if (item.Length == 0)
                    {
                        continue;
                    }
                    var newURL = GenerateRandomString(10) + Path.GetExtension(item.FileName);
                    string Name = Path.GetFileNameWithoutExtension(item.FileName);

                    int counter = 1;

                    while (_context.keeper_imgs.Where(t => t.img_type_id == typeid).Any(n => n.name == Name))
                    {
                        Name = $"{Path.GetFileNameWithoutExtension(item.FileName)}_{counter}";
                        counter++;
                    }
                    var newKeeperImg = new keeper_img
                    {
                        img_type_id = typeid,
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
            }
            catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }
        }
        private async Task save_Bimg_intype(int userid, int typeid, List<IFormFile> imgs)
        {
            try
            {
                foreach (var item in imgs)
                {
                    if (item.Length == 0)
                    {
                        continue;
                    }
                    var newURL = GenerateRandomString(10) + Path.GetExtension(item.FileName);
                    string Name = Path.GetFileNameWithoutExtension(item.FileName);

                    int counter = 1;

                    while (_context.business_imgs.Where(t => t.img_type_id == typeid).Any(n => n.name == Name))
                    {
                        Name = $"{Path.GetFileNameWithoutExtension(item.FileName)}_{counter}";
                        counter++;
                    }
                    var newBusinesImg = new business_img
                    {
                        img_type_id = typeid,
                        URL = newURL,
                        name = Name,
                    };
                    _context.Add(newBusinesImg);
                    await _context.SaveChangesAsync();
                    //2.2.3 存入圖片到本地指定資料夾

                    var path = Path.Combine(
                            Directory.GetCurrentDirectory(), "wwwroot/imgs/business_img", newURL);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await item.CopyToAsync(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        [HttpGet("GetAllpost")]
        public async Task<IActionResult> GetAllpost()
        {
            //獲得最新的幾天
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
