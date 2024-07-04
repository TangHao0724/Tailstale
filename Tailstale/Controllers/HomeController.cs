using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Tailstale.Models;
using Tailstale.partial;

namespace Tailstale.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
      //  private readonly TailstaleContext _context;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
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
        //public IActionResult CreateBooking()
        //{
        //    ViewBag.Hotel = new SelectList(_context.businesses.Select(h => new {
        //        HotelId = h.ID,
        //        HotelName = h.name,
        //    }), "HotelId", "HotelName");
        //    return View();
        //}
    }
}
