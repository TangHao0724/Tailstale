using Microsoft.AspNetCore.Mvc;

namespace Tailstale.Controllers
{
    public class UserInfoController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.loginID = HttpContext.Session.GetInt32("loginID");
            return View();
        }
    }
}
