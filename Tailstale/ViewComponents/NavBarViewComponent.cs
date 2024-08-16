using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using System.Drawing.Imaging;
using System.Security.Policy;
using Tailstale.Models;
using Tailstale.Tools;

namespace Tailstale.ViewComponents
{
    public class NavBarViewComponent : ViewComponent
    {
        private readonly TailstaleContext _context;

        public NavBarViewComponent(TailstaleContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
           int? ids = HttpContext.Session.GetInt32("loginID");
            int? UTypes = HttpContext.Session.GetInt32("loginType");
            string? url;
            if (UTypes != 0)
            {
                url = _context.businesses.Where(n => n.ID == ids).Select(n => n.photo_url).FirstOrDefault();
            }
            else
            {
                int imgtypeld = _context.keeper_img_types.Where(a => a.FK_Keeper_id == ids && a.typename == $"{ids}_head").Select(s => s.ID).FirstOrDefault();
                url = _context.keeper_imgs.Where(a => a.img_type_id == imgtypeld && a.name.Contains("head"))
                            .OrderByDescending(x => x.created_at)
                            .Select(s => s.URL)
                            .FirstOrDefault();
            }
            
            //分辨輸入ID。Usertype
            // 根據 ID 執行你的邏輯，例如從資料庫中獲取資訊
            // 傳遞資訊到 View
            switch (UTypes)
            {
                case 0:
                    ViewBag.Keeper_imgurl = url != null ? $"/imgs/keeper_img/{url}" : "/imgs/keeper_img/no_head.png";
                    break;
                case 1://旅館
                    string hotel_imgurl = "images/business/";
                    ViewBag.hotel_imgurl = url != null ?hotel_imgurl+url : "/imgs/keeper_img/no_head.png";
                    break;
                case 2://美容
                    string salon_imgurl = " https://localhost:7112/Salon_img/";
                    ViewBag.salon_imgurl = url != null ? salon_imgurl+url : "/imgs/keeper_img/no_head.png";

                    break;
                case 3://醫院
                    string hospital_imgurl = " https://localhost:7112/lib/HospitalImages";
                    ViewBag.hospital_imgurl = url != null ? hospital_imgurl+url : "/imgs/keeper_img/no_head.png";

                    break;
            }
            ViewBag.UserID = ids;
            ViewBag.UType = UTypes;
            return View("_NavBar");
        }
    }
}
