using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Xml.Linq;
using Tailstale.Models;

namespace Tailstale.Controllers
{
    public class UserController : Controller
    {
        private readonly TailstaleContext _context;

        public UserController(TailstaleContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            ViewBag.loginID = HttpContext.Session.GetInt32("loginID");
            ViewBag.loginType = HttpContext.Session.GetInt32("loginType");
            int? loginType = HttpContext.Session.GetInt32("loginType");
            int? loginID = HttpContext.Session.GetInt32("loginID");

            string? url;

            if (loginType != 0)
            {
                url = _context.businesses.Where(n => n.ID == loginID).Select(n => n.photo_url).FirstOrDefault();
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
            return View();
        }
        public IActionResult search()
        {
            ViewBag.loginID = HttpContext.Session.GetInt32("loginID");
            ViewBag.loginType = HttpContext.Session.GetInt32("loginType");
            return View();
        }
    }

}
