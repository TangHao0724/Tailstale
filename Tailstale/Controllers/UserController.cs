using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace Tailstale.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.loginID = HttpContext.Session.GetInt32("loginID");
            ViewBag.loginType = HttpContext.Session.GetInt32("loginType");
            return View();
        }
        public IActionResult social()
        {
            ViewBag.loginID = HttpContext.Session.GetInt32("loginID");
            ViewBag.loginType = HttpContext.Session.GetInt32("loginType");
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
