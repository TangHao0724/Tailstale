using CRUD_COREMVC;
using Microsoft.AspNetCore.Mvc;
using Tailstale.Models;

namespace Tailstale.Controllers
{
   
    public class SalonController : Controller
    {

        private readonly ILogger<SalonController> _logger;
        private readonly TailstaleContext _context;

        //  private readonly TailstaleContext _context;

        public SalonController(ILogger<SalonController> logger, TailstaleContext context)
        {
            _logger = logger;
            _context = context;
        }

        [IsLoginFilter]
        public IActionResult Settings()
        {
            return View();
        }

        public IActionResult SalonHome()
        {
            return View();
        }
    }
}
