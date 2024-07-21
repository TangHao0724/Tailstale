using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace Tailstale.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.userID = HttpContext.Session.GetInt32("loginID");
            ViewBag.userType = HttpContext.Session.GetInt32("loginType");

            return View();
        }
    }

}
