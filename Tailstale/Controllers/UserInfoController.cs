using Microsoft.AspNetCore.Mvc;

namespace Tailstale.Controllers
{
    public class UserInfoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
