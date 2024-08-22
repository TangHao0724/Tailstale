using Microsoft.AspNetCore.Mvc;
using System.Security.Policy;
using Tailstale.Models;

namespace Tailstale.ViewComponents
{
    public class PostViewComponent : ViewComponent
    {
        private readonly TailstaleContext _context;

        public PostViewComponent(TailstaleContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            int? loginId = HttpContext.Session.GetInt32("loginID");
            int? loginType = HttpContext.Session.GetInt32("loginType");
            ViewBag.userID = loginId;
            string? url;
            if (loginType != 0)
            {
                url = _context.businesses.Where(n => n.ID == loginId).Select(n => n.photo_url).FirstOrDefault();
            }
            else
            {   
                int imgtypeld = _context.keeper_img_types.Where(a => a.FK_Keeper_id == loginId && a.typename == $"{loginId}_head").Select(s => s.ID).FirstOrDefault();
                url = _context.keeper_imgs.Where(a => a.img_type_id == imgtypeld && a.name.Contains("head"))
                            .OrderByDescending(x => x.created_at)
                            .Select(s => s.URL)
                            .FirstOrDefault();
            }
            switch (loginType)
            {
                case 1://旅館
                    string hotel_imgurl = "images/business/";
                    ViewBag.hotel_imgurl = url != null ? hotel_imgurl + url : "imgs/keeper_img/no_head.png";
                    break;
                case 2://美容
                    string salon_imgurl = "https://localhost:7112/Salon_img/";
                    ViewBag.salon_imgurl = url != null ? salon_imgurl + url : "imgs/keeper_img/no_head.png";
                    break;
                case 3://醫院
                    string hospital_imgurl = "lib/HospitalImages/";
                    ViewBag.hospital_imgurl = url != null ? hospital_imgurl + url : "imgs/keeper_img/no_head.png";
                    break;
                default:
                    ViewBag.Keeper_imgurl = url != null ? $"imgs/keeper_img/{url}" : "imgs/keeper_img/no_head.png";
                    break;
            }
            return  View("_post");
        }
    }
}
