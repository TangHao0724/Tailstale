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

            ViewBag.loginID = HttpContext.Session.GetInt32("loginID");
            ViewBag.loginType = HttpContext.Session.GetInt32("loginType");

            ViewBag.loginName = _context.keepers.Where(m => m.ID == HttpContext.Session.GetInt32("loginID"))
                                .Select(n => n.name)
                                .FirstOrDefault();


            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
