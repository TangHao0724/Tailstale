using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Security.Policy;
using Tailstale.Models;

namespace Tailstale.ViewComponents
{
    public class LUTagViewComponent : ViewComponent
    {
        private readonly TailstaleContext _context;

        public LUTagViewComponent(TailstaleContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            int? loginType = HttpContext.Session.GetInt32("loginType");
            int? loginId = HttpContext.Session.GetInt32("loginID");
            string? userName = null;

            if (loginType != null && loginId != null)
            {
                if (loginType == 0)
                {
                    userName = await _context.keepers
                        .Where(k => k.ID == loginId)
                        .Select(k => k.name)
                        .FirstOrDefaultAsync();
                }
                else
                {
                    userName = await _context.businesses
                        .Where(k => k.ID == loginId)
                        .Select(k => k.name)
                        .FirstOrDefaultAsync();
                }
            }
            string? url;
            if (loginType != 0)
            {
                url = _context.businesses.Where(n => n.ID == loginId).Select(n => n.photo_url).FirstOrDefault();
            }
            else
            {
                url = null;
            }
            switch (loginType)
            {
                case 1://旅館
                    string hotel_imgurl = "";
                    ViewBag.hotel_imgurl = url != null ? hotel_imgurl + url : "/imgs/keeper_img/no_head.png";
                    break;
                case 2://美容
                    string salon_imgurl = "";
                    ViewBag.salon_imgurl = url != null ? salon_imgurl + url : "/imgs/keeper_img/no_head.png";
                    break;
                case 3://醫院
                    string hospital_imgurl = "";
                    ViewBag.hospital_imgurl = url != null ? hospital_imgurl + url : "/imgs/keeper_img/no_head.png";
                    break;
                default:
                    ViewBag.Keeper_imgurl = "/imgs/keeper_img/no_head.png";
                    break;
            }

            ViewBag.userID = loginId;
            ViewBag.userType = loginType;
            ViewBag.userName = userName;

            return View("_LUTag");
        }
    }
}
