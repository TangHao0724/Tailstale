using Humanizer;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Tailstale.Models;

namespace Tailstale.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TailstaleContext _context;

        public HomeController(ILogger<HomeController> logger,TailstaleContext context )
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {

            ViewBag.loginID = HttpContext.Session.GetString("loginID");
            ViewBag.loginType = HttpContext.Session.GetString("loginType");
            //Âà¬°int
            int intID = Convert.ToInt32(HttpContext.Session.GetString("loginID"));
            ViewBag.loginName = _context.keepers.Where(m => m.ID == intID).Select(n => n.name).FirstOrDefault();


            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
