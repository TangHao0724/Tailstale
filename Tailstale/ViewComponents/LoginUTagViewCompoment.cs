using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using System.Drawing.Imaging;
using System.Security.Policy;
using Tailstale.Models;

namespace Tailstale.ViewComponents
{
    public class LoginUTagViewComponent : ViewComponent
    {
        private readonly TailstaleContext _context;

        public LoginUTagViewComponent(TailstaleContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(int id,int UType)
        {
            string? url;
            if (UType != 0)
            {
                url = _context.businesses.Where(n => n.ID == id).Select(n => n.photo_url).FirstOrDefault();
            }
            else
            {
                int imgtypeld = _context.keeper_img_types.Where(a => a.FK_Keeper_id == id && a.typename == $"{id}_head").Select(s => s.ID).FirstOrDefault();
                url = _context.keeper_imgs.Where(a => a.img_type_id == imgtypeld && a.name.Contains("head"))
                            .OrderByDescending(x => x.created_at)
                            .Select(s => s.URL)
                            .FirstOrDefault();
            }
            
            //分辨輸入ID。Usertype
            // 根據 ID 執行你的邏輯，例如從資料庫中獲取資訊
            // 傳遞資訊到 View
            switch (UType)
            {
                
                case 1://旅館
                    string hotel_imgurl = "";
                    ViewBag.hotel_imgurl = url != null ?hotel_imgurl+url : "/imgs/keeper_img/no_head.png";
                    ViewBag.UserID = id;
                    ViewBag.UType = UType;
                    return View("_LoginUTagHotel");
                case 2://美容
                    string salon_imgurl = "Salon_img/";
                    ViewBag.salon_imgurl = url != null ? salon_imgurl+url : "/imgs/keeper_img/no_head.png";
                    ViewBag.UserID = id;
                    ViewBag.UType = UType;
                    return View("_LoginUTagSalon");
                case 3://醫院
                    string hospital_imgurl = "";
                    ViewBag.hospital_imgurl = url != null ? hospital_imgurl+url : "/imgs/keeper_img/no_head.png";
                    ViewBag.UserID = id;
                    ViewBag.UType = UType;
                    return View("_LoginUTagHospital");
                default:
                    
                    ViewBag.Keeper_imgurl = url !=null? $"/imgs/keeper_img/{url}" : "/imgs/keeper_img/no_head.png";
                    ViewBag.UserID = id;
                    return View("_LoginUTagKeeper");
            }

        }
    }
}
