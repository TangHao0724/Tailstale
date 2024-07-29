using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Tailstale.Models;

namespace Tailstale.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TailstaleContext _context;


        public HomeController(ILogger<HomeController> logger, TailstaleContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var r = _context.keepers.Select(k => k).ToList();
            return View(r);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
