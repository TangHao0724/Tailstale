using Microsoft.AspNetCore.Mvc;

namespace Tailstale.Controllers
{
    public class LNRController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }
        public async Task<IActionResult> BLogin()
        {
            return View();
        }
        public async Task<IActionResult> KRegister()
        {
            return View();
        }
        public async Task<IActionResult> BRegister()
        {
            return View();
        }
    }
}
