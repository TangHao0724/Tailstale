﻿using Azure;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
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
                bool isKeeper = DTO.Keeper_ID != null && DTO.Business_ID == null;
                bool isBusiness = DTO.Business_ID != null && DTO.Keeper_ID == null;

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
                if (isKeeper)
                {
                    newArticle.FK_Keeper_ID = DTO.Keeper_ID;
                }
                else if (isBusiness)
                {
                    newArticle.FK_Business_ID = DTO.Business_ID;
                }
                else
                {
                    return BadRequest("發文者身份不明");
                }

                newArticle.content = DTO.Content;
                newArticle.ispublic = DTO.isPublic ?? false;

                _context.Add(newArticle);
                await _context.SaveChangesAsync();

                /*存放tag
                1. 檢查是否有相同ID的tag
                2.若有，不儲存，有責存入該ID
                3.存入後，在using_tag建立與newArticle.id2的一筆資料
                 */
                if (DTO.PublicTags != null && DTO.PublicTags.Count > 0)
                {
                    string[] publicTagsArray = DTO.PublicTags[0].Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string item in publicTagsArray)
                    {
                        var trimmedItem = item.Trim();
                        // 如果沒有相同，建立新資料
                        if (!await _context.tags.AnyAsync(n => n.name == trimmedItem))
                        {
                            _context.Add(new tag { name = trimmedItem });

                        }
                        await _context.SaveChangesAsync();
                        // 檢索tag.name的ID，將其與usingtag相連
                        var tagID = await _context.tags.Where(n => n.name == trimmedItem).Select(s => s.ID).FirstOrDefaultAsync();
                        _context.Add(new using_tag
                        {
                            FK_article_ID = newArticle.ID,
                            FK_tags_ID = tagID
                        });
                    }
                    await _context.SaveChangesAsync();
                }
                if (DTO.PrivateTags != null && DTO.PrivateTags.Count > 0)
                {
                    string[] PrivateTagsArray = DTO.PrivateTags[0].Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                    if (isKeeper)
                    {
                        await ProcessPrivateTags(PrivateTagsArray, DTO.Keeper_ID, null, newArticle.ID);
                    }
                    if (isBusiness)
                    {
                        await ProcessPrivateTags(PrivateTagsArray, null, DTO.Business_ID, newArticle.ID);
                    }
                }

                // 判斷是否有圖片
                if (DTO.imgs == null || DTO.imgs.Count == 0)
                {
                    return Ok(new { Message = $"PO文成功!沒有圖片 ID{newArticle.ID}" });
                }

                // 處理圖片類型
                if (isKeeper)
                {
                    await ProcessImageTypes(DTO.Keeper_ID, null, DTO.imgs, article_typename, newArticle.ID);
                }
                else if (isBusiness)
                {
                    await ProcessImageTypes(null, DTO.Business_ID, DTO.imgs, article_typename, newArticle.ID);
                }

                await _context.SaveChangesAsync();

                return Ok(new { Message = $"PO文成功!包含圖片 ID{newArticle.ID}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"上傳圖片失敗: {ex.Message}" });
            }
        }

        //從parentID查 父文章
        [HttpGet("GetArticle")]
        public async Task<IActionResult> GetArticle(int? count,int? id,int? parentid,bool? publicOnly,int? artID)
        {
            
            try
            {
                List<article> articles = new List<article>();
                // 如果要求數比文章多
                //判斷公開性
                //判段是否有userid指定用戶的貼文
                //判斷從屬文章
                //判斷數量

                //選擇自己的私人文章+
                // 先抓指定數量文章
                articles = await _context.articles
                                         .OrderByDescending(x => x.created_at)
                                         .Take(count ?? int.MaxValue) // 取指定數量或全部（如果count為null）
                                         .ToListAsync();
                if (parentid.HasValue)
                {
                    if (parentid == 0)
                    {
                        articles = articles.Where(n => n.parent_ID == null).ToList();
                    }
                    else
                    {
                        articles = articles.Where(n => n.parent_ID == parentid).ToList();
                    }
                }
                if (artID.HasValue)
                {
                    articles = articles.Where(n=>n.ID == artID).ToList();
                }
                // 根據publicOnly標誌過濾
                if (publicOnly.HasValue)
                {
                    if (publicOnly == true)
                    {
                        articles = articles.Where(n => n.ispublic == true).ToList();
                    }
                    else if(publicOnly == false)
                    {
                        articles = articles.Where(n => n.ispublic == false).ToList();
                    }

                }

                // 如果提供了用戶ID，則過濾
                if (id.HasValue)
                {
                    articles = articles.Where(n => n.FK_Keeper_ID == id.Value).ToList();
                }
                
                // 如果count超過文章數量，則調整count
                if (count.HasValue && count.Value < articles.Count)
                {
                    count = articles.Count;
                }
                if (count.HasValue) 
                {
                    articles = articles.Take(count.Value).ToList();
                }


                // 抓取所有相關資料
                var articleImgs = await _context.article_imgs.ToListAsync();
                var keeperImgs = await _context.keeper_imgs.ToListAsync();
                var businessImgs = await _context.business_imgs.ToListAsync();
                var business = await _context.businesses.ToListAsync();
                var keeper = await _context.keepers.ToListAsync();
                var publictag = await _context.tags.ToListAsync();
                var using_pubtag = await _context.using_tags.ToListAsync();
                var pritag = await _context.person_tags.ToListAsync();
                var using_pritag = await _context.using_person_tags.ToListAsync();
                var imgType = await _context.keeper_img_types.ToListAsync();
                var img = await _context.keeper_imgs.ToListAsync();

                var withImg = articles.Select(n =>
                {
                // 圖片陣列
                var imgurllist = articleImgs
                    .Where(a => a.FK_article_ID == n.ID)
                    .SelectMany(a =>
                        a.FK_Business_img_ID != null
                        ? businessImgs.Where(b => b.ID == a.FK_Business_img_ID).Select(b => b.URL)
                        : keeperImgs.Where(k => k.ID == a.FK_Keeper_img_ID).Select(k => k.URL)
                    ).ToList();

                var BName = business.Where(a => a.ID == n.FK_Business_ID).Select(s => s.name).FirstOrDefault();
                var KName = keeper.Where(a => a.ID == n.FK_Keeper_ID).Select(s => s.name).FirstOrDefault();

                // 公開TAG
                var using_publicTaglist = using_pubtag.Where(a => a.FK_article_ID == n.ID).Select(s => s.FK_tags_ID).ToList();
                List<string?> publicTaglist = new List<string?>();

                foreach (int id in using_publicTaglist)
                {
                    string? tagn = publictag.Where(a => a.ID == id).Select(s => s.name).FirstOrDefault();
                    publicTaglist.Add(tagn);
                }

                // 私人TAG
                var usingk_privateTaglist = using_pritag.Where(a => a.FK_article_ID == n.ID && a.FK_Keeper_ID == n.FK_Keeper_ID).Select(s => s.FK_person_tags_ID).ToList();
                var usingb_privateTaglist = using_pritag.Where(a => a.FK_article_ID == n.ID && a.FK_Business_ID == n.FK_Business_ID).Select(s => s.FK_person_tags_ID).ToList();
                List<string?> privateTaglist = new List<string?>();

                if (usingk_privateTaglist.Any())
                {
                    foreach (int id in usingk_privateTaglist)
                    {
                        string? tagn = pritag.Where(a => a.ID == id && a.FK_Keeper_ID == n.FK_Keeper_ID).Select(s => s.name).FirstOrDefault();
                        privateTaglist.Add(tagn);
                    }

                    }
                else if(usingb_privateTaglist.Any())
                {
                    foreach (int id in usingb_privateTaglist)
                    {
                        string? tagn = pritag.Where(a => a.ID == id && a.FK_Business_ID == n.FK_Business_ID).Select(s => s.name).FirstOrDefault();
                        privateTaglist.Add(tagn);
                            
                    }
                        
                }
                //抓取使用者頭貼
                string imgurl = "no_head.png";
                int uType = 0;
                if (n.FK_Keeper_ID != null)
                {
                    //只會有顯示目前USER的HEADURL
                    int imgtypeld = imgType.Where(a => a.FK_Keeper_id == n.FK_Keeper_ID && a.typename == $"{n.FK_Keeper_ID}_head").Select(s => s.ID).FirstOrDefault();
                    imgurl = img.Where(a => a.img_type_id == imgtypeld && a.name.Contains("head"))
                                .OrderByDescending(x => x.created_at)
                                .Select(s => s.URL)
                                .FirstOrDefault() ?? "no_head.png";
                }
                else
                {
                    imgurl = business.Find(a => a.ID == n.FK_Business_ID).photo_url ?? "no_head.png";
                    uType = business.Find(a => a.ID == n.FK_Business_ID).type_ID ?? 0;
                }



                    // 返回結果
                    return new
                    {
                        BName,
                        KName,
                        privateTaglist,
                        uType,
                        imgurl,
                        n.ispublic,
                        n.created_at,
                        n.parent_ID,
                        n.ID,
                        n.content,
                        imgurllist,
                        publicTaglist
                    };
                }).ToList();

                // 返回結果
                return new JsonResult(withImg);
            }
            catch (Exception ex)
            {
                // 錯誤處理
                return StatusCode(500, $"內部伺服器錯誤: {ex.Message}");
            }
        }

        //列出個人TAG
        [HttpGet("GetPriTagList")]
        public async Task<IActionResult> GetPriTagList(int id ,int UType)
        {

            if(UType == 0)
            { 
              var tagKCounts = _context.using_person_tags
                                        .Where(ut => ut.FK_Keeper_ID == id)
                                        .Select(ut => new
                                        {
                                            TagName = ut.FK_person_tags.name,
                                            Count = 1
                                        })
                                        .ToList();

                if (!tagKCounts.Any())
                {
                    return NoContent(); // 或者返回其他適當的訊息
                }

                var result = tagKCounts
                            .GroupBy(t => t.TagName)
                            .Select(g => new
                            {
                                TagName = g.Key,
                                Count = g.Count()
                            })
                            .OrderByDescending(c => c.Count)
                            .ToList();

            return Ok(result);  
            }
            var tagKCountsa = _context.using_person_tags
                                      .Where(ut => ut.FK_Business_ID == id)
                                      .Select(ut => new
                                      {
                                          TagName = ut.FK_Business_ID,
                                          Count = 10
                                      })
                                      .ToList();

            if (!tagKCountsa.Any())
            {
                return NoContent(); // 或者返回其他適當的訊息
            }

            var resulta = tagKCountsa
                        .GroupBy(t => t.TagName)
                        .Select(g => new
                        {
                            TagName = g.Key,
                            Count = g.Count()
                        })
                        .OrderByDescending(c => c.Count)
                        .ToList();

            return Ok(resulta);

        }
        //輸入個人TAG、查詢包含個人TAG的文章
        [HttpGet("GetPriTagArt")]
        public async Task<IActionResult> GetPriTagArt(string priTag, int userID, int UType)
        {
            //以tagID、以及使用者ID去搜尋含有tagID 的using_tag 
            //再用article去搜尋 文章using_tag的FKID
            var personTagQuery = _context.person_tags
                                         .Where(m => m.name == priTag &&
                                                     (UType == 0 ? m.FK_Keeper_ID == userID : m.FK_Business_ID == userID))
                                         .Select(s => s.ID)
                                         .FirstOrDefault();

            var articles = await (from a in _context.articles
                                  join ut in _context.using_person_tags
                                  on a.ID equals ut.FK_article_ID
                                  where ut.FK_person_tags_ID == personTagQuery
                                  select a).ToListAsync();
            // 抓取所有相關資料
            var articleImgs = await _context.article_imgs.ToListAsync();
            var keeperImgs = await _context.keeper_imgs.ToListAsync();
            var businessImgs = await _context.business_imgs.ToListAsync();
            var business = await _context.businesses.ToListAsync();
            var keeper = await _context.keepers.ToListAsync();
            var publictag = await _context.tags.ToListAsync();
            var using_pubtag = await _context.using_tags.ToListAsync();
            var pritag = await _context.person_tags.ToListAsync();
            var using_pritag = await _context.using_person_tags.ToListAsync();
            var imgType = await _context.keeper_img_types.ToListAsync();
            var img = await _context.keeper_imgs.ToListAsync();

            var withImg = articles.Select(n =>
            {
                // 圖片陣列
                var imgurllist = articleImgs
                    .Where(a => a.FK_article_ID == n.ID)
                    .SelectMany(a =>
                        a.FK_Business_img_ID != null
                        ? businessImgs.Where(b => b.ID == a.FK_Business_img_ID).Select(b => b.URL)
                        : keeperImgs.Where(k => k.ID == a.FK_Keeper_img_ID).Select(k => k.URL)
                    ).ToList();

                var BName = business.Where(a => a.ID == n.FK_Business_ID).Select(s => s.name).FirstOrDefault();
                var KName = keeper.Where(a => a.ID == n.FK_Keeper_ID).Select(s => s.name).FirstOrDefault();

                // 公開TAG
                var using_publicTaglist = using_pubtag.Where(a => a.FK_article_ID == n.ID).Select(s => s.FK_tags_ID).ToList();
                List<string?> publicTaglist = new List<string?>();

                foreach (int id in using_publicTaglist)
                {
                    string? tagn = publictag.Where(a => a.ID == id).Select(s => s.name).FirstOrDefault();
                    publicTaglist.Add(tagn);
                }

                // 私人TAG
                var usingk_privateTaglist = using_pritag.Where(a => a.FK_article_ID == n.ID && a.FK_Keeper_ID == n.FK_Keeper_ID).Select(s => s.FK_person_tags_ID).ToList();
                var usingb_privateTaglist = using_pritag.Where(a => a.FK_article_ID == n.ID && a.FK_Business_ID == n.FK_Business_ID).Select(s => s.FK_person_tags_ID).ToList();
                List<string?> privateTaglist = new List<string?>();

                if (usingk_privateTaglist.Any())
                {
                    foreach (int id in usingk_privateTaglist)
                    {
                        string? tagn = pritag.Where(a => a.ID == id && a.FK_Keeper_ID == n.FK_Keeper_ID).Select(s => s.name).FirstOrDefault();
                        privateTaglist.Add(tagn);
                    }
                }
                else if (usingb_privateTaglist.Any())
                {
                    foreach (int id in usingb_privateTaglist)
                    {
                        string? tagn = pritag.Where(a => a.ID == id && a.FK_Business_ID == n.FK_Business_ID).Select(s => s.name).FirstOrDefault();
                        privateTaglist.Add(tagn);
                    }
                }
                //抓取使用者頭貼
                string imgurl = "no_head.png";
                int uType = 0;
                if (n.FK_Keeper_ID != null)
                {
                    //只會有顯示目前USER的HEADURL
                    int imgtypeld = imgType.Where(a => a.FK_Keeper_id == n.FK_Keeper_ID && a.typename == $"{n.FK_Keeper_ID}_head").Select(s => s.ID).FirstOrDefault();
                    imgurl = img.Where(a => a.img_type_id == imgtypeld && a.name.Contains("head"))
                                .OrderByDescending(x => x.created_at)
                                .Select(s => s.URL)
                                .FirstOrDefault() ?? "no_head.png";
                }
                else
                {
                    imgurl = business.Find(a => a.ID == n.FK_Business_ID).photo_url ?? "no_head.png";
                    uType = business.Find(a => a.ID == n.FK_Business_ID).type_ID ?? 0;
                }

                // 返回結果
                return new
                {
                    BName,
                    KName,
                    privateTaglist,
                    uType,
                    imgurl,
                    n.ispublic,
                    n.created_at,
                    n.parent_ID,
                    n.ID,
                    n.content,
                    imgurllist,
                    publicTaglist
                };
            }).ToList();

            return Ok(withImg);
        }
        //列出主流TAG
        [HttpGet("GetPubTagList")]
        public async Task<IActionResult> GetPubTagList()
        {

            //統計最近一周內所有TAG_ID產生各自的數量，
            //排序使用次數
            //由上到下排序數量
            //如果為空，則停止使用
            var WeekDate = DateTime.Now.AddDays(-7);
            var tagCounts = await _context.using_tags
                                          .Where(ut => ut.created_at >= WeekDate)
                                          .GroupBy(ut => new { ut.FK_tags_ID, ut.FK_tags.name })
                                          .Select(g => new
                                          {
                                              TagName = g.Key.name,
                                              Count = g.Count()
                                          })
                                          .OrderByDescending(c => c.Count)
                                          .ToListAsync();
            if (!tagCounts.Any())
            {
                return NoContent(); // 或者返回其他適當的訊息
            }
            return Ok(tagCounts);
        }
        //輸入TAG字串，查詢包含TAG的文章
        [HttpGet("GetPubTagArt")]
        public async Task<IActionResult> GetPubTagArt(string Tag)
        {
            var TagQuery = _context.tags
                             .Where(m => m.name == Tag)
                             .Select(s => s.ID)
                             .FirstOrDefault();

            var articles = await (from a in _context.articles
                                  join ut in _context.using_tags
                                  on a.ID equals ut.FK_article_ID
                                  where ut.FK_tags_ID == TagQuery
                                  select a).ToListAsync();

            // 抓取所有相關資料
            var articleImgs = await _context.article_imgs.ToListAsync();
            var keeperImgs = await _context.keeper_imgs.ToListAsync();
            var businessImgs = await _context.business_imgs.ToListAsync();
            var business = await _context.businesses.ToListAsync();
            var keeper = await _context.keepers.ToListAsync();
            var publictag = await _context.tags.ToListAsync();
            var using_pubtag = await _context.using_tags.ToListAsync();
            var pritag = await _context.person_tags.ToListAsync();
            var using_pritag = await _context.using_person_tags.ToListAsync();
            var imgType = await _context.keeper_img_types.ToListAsync();
            var img = await _context.keeper_imgs.ToListAsync();

            var withImg = articles.Select(n =>
            {
                // 圖片陣列
                var imgurllist = articleImgs
                    .Where(a => a.FK_article_ID == n.ID)
                    .SelectMany(a =>
                        a.FK_Business_img_ID != null
                        ? businessImgs.Where(b => b.ID == a.FK_Business_img_ID).Select(b => b.URL)
                        : keeperImgs.Where(k => k.ID == a.FK_Keeper_img_ID).Select(k => k.URL)
                    ).ToList();

                var BName = business.Where(a => a.ID == n.FK_Business_ID).Select(s => s.name).FirstOrDefault();
                var KName = keeper.Where(a => a.ID == n.FK_Keeper_ID).Select(s => s.name).FirstOrDefault();

                // 公開TAG
                var using_publicTaglist = using_pubtag.Where(a => a.FK_article_ID == n.ID).Select(s => s.FK_tags_ID).ToList();
                List<string?> publicTaglist = new List<string?>();

                foreach (int id in using_publicTaglist)
                {
                    string? tagn = publictag.Where(a => a.ID == id).Select(s => s.name).FirstOrDefault();
                    publicTaglist.Add(tagn);
                }

                // 私人TAG
                var usingk_privateTaglist = using_pritag.Where(a => a.FK_article_ID == n.ID && a.FK_Keeper_ID == n.FK_Keeper_ID).Select(s => s.FK_person_tags_ID).ToList();
                var usingb_privateTaglist = using_pritag.Where(a => a.FK_article_ID == n.ID && a.FK_Business_ID == n.FK_Business_ID).Select(s => s.FK_person_tags_ID).ToList();
                List<string?> privateTaglist = new List<string?>();

                if (usingk_privateTaglist.Any())
                {
                    foreach (int id in usingk_privateTaglist)
                    {
                        string? tagn = pritag.Where(a => a.ID == id && a.FK_Keeper_ID == n.FK_Keeper_ID).Select(s => s.name).FirstOrDefault();
                        privateTaglist.Add(tagn);
                    }
                }
                else if (usingb_privateTaglist.Any())
                {
                    foreach (int id in usingb_privateTaglist)
                    {
                        string? tagn = pritag.Where(a => a.ID == id && a.FK_Business_ID == n.FK_Business_ID).Select(s => s.name).FirstOrDefault();
                        privateTaglist.Add(tagn);
                    }
                }
                //抓取使用者頭貼
                string imgurl = "no_head.png";
                int uType = 0;
                if (n.FK_Keeper_ID != null)
                {
                    //只會有顯示目前USER的HEADURL
                    int imgtypeld = imgType.Where(a => a.FK_Keeper_id == n.FK_Keeper_ID && a.typename == $"{n.FK_Keeper_ID}_head").Select(s => s.ID).FirstOrDefault();
                    imgurl = img.Where(a => a.img_type_id == imgtypeld && a.name.Contains("head"))
                                .OrderByDescending(x => x.created_at)
                                .Select(s => s.URL)
                                .FirstOrDefault() ?? "no_head.png";
                }
                else
                {
                    imgurl = business.Find(a => a.ID == n.FK_Business_ID).photo_url ?? "no_head.png";
                    uType = business.Find(a => a.ID == n.FK_Business_ID).type_ID ?? 0;
                }

                // 返回結果
                return new
                {
                    BName,
                    KName,
                    privateTaglist,
                    uType,
                    imgurl,
                    n.ispublic,
                    n.created_at,
                    n.parent_ID,
                    n.ID,
                    n.content,
                    imgurllist,
                    publicTaglist
                };
            }).ToList();

            return Ok(withImg);

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

        private async Task ProcessPrivateTags(string[] tags, int? keeperId, int? businessId, int articleId)
        {
            foreach (string item in tags)
            {
                var trimmedItem = item.Trim();
                // 如果沒有相同，建立新資料
                if (keeperId.HasValue && !await _context.person_tags.Where(n => n.FK_Keeper_ID == keeperId.Value).AnyAsync(n => n.name == trimmedItem))
                {
                    
                    _context.Add(new person_tag { name = trimmedItem, FK_Keeper_ID = keeperId.Value });
                    await _context.SaveChangesAsync();
                }
                else if (businessId.HasValue && !await _context.person_tags.Where(n => n.FK_Business_ID == businessId.Value).AnyAsync(n => n.name == trimmedItem))
                {
                    _context.Add(new person_tag { name = trimmedItem, FK_Business_ID = businessId.Value });
                    await _context.SaveChangesAsync();
                }
                

                // 檢索tag.name的ID，將其與usingtag相連
                var tagname = await _context.person_tags
                    .Where(n => ((keeperId.HasValue && n.FK_Keeper_ID == keeperId.Value) ||
                     (businessId.HasValue && n.FK_Business_ID == businessId.Value)) &&
                     n.name == trimmedItem)
                    .Select(s => s.ID)
                    .FirstOrDefaultAsync();

                _context.Add(new using_person_tag
                {
                    FK_article_ID = articleId,
                    FK_person_tags_ID = tagname,
                    FK_Keeper_ID = keeperId,
                    FK_Business_ID = businessId
                });
            }
            await _context.SaveChangesAsync();
        }
        private async Task ProcessImageTypes(int? keeperId, int? businessId, List<IFormFile> imgs, string articleTypename, int articleId)
        {
            int typeId;

            if (keeperId.HasValue)
            {
                if (!_context.keeper_img_types.Where(n => n.FK_Keeper_id == keeperId.Value).Any(x => x.typename.Equals(articleTypename)))
                {
                    var newKeeperImgType = new keeper_img_type
                    {
                        FK_Keeper_id = keeperId.Value,
                        typename = articleTypename,
                    };
                    _context.Add(newKeeperImgType);
                    await _context.SaveChangesAsync();
                }
                typeId = await _context.keeper_img_types.Where(n => n.FK_Keeper_id == keeperId.Value && n.typename.Equals(articleTypename)).Select(s => s.ID).FirstAsync();
                await save_Kimg_intype(keeperId.Value, typeId, imgs);

                var imgsName = await _context.keeper_imgs.Where(f => f.img_type_id == typeId).Select(s => s.ID).ToListAsync();
                var articleImgs = imgsName.Select(img => new article_img
                {
                    FK_article_ID = articleId,
                    FK_Keeper_img_ID = img
                }).ToList();

                _context.article_imgs.AddRange(articleImgs);
            }
            else if (businessId.HasValue)
            {
                if (!_context.business_img_types.Where(n => n.FK_business_id == businessId.Value).Any(x => x.typename.Equals(articleTypename)))
                {
                    var newBusinessImgType = new business_img_type
                    {
                        FK_business_id = businessId.Value,
                        typename = articleTypename,
                    };
                    _context.Add(newBusinessImgType);
                    await _context.SaveChangesAsync();
                }
                typeId = await _context.business_img_types.Where(n => n.FK_business_id == businessId.Value && n.typename.Equals(articleTypename)).Select(s => s.ID).FirstAsync();
                await save_Bimg_intype(businessId.Value, typeId, imgs);

                var imgsName = await _context.business_imgs.Where(f => f.img_type_id == typeId).Select(s => s.ID).ToListAsync();
                var articleImgs = imgsName.Select(img => new article_img
                {
                    FK_article_ID = articleId,
                    FK_Business_img_ID = img
                }).ToList();

                _context.article_imgs.AddRange(articleImgs);
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


        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}
