using CRUD_COREMVC;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public IActionResult SalonWork()
        {
            return View();
        }

        public IActionResult SalonHome()
        {

            ViewData["pet_name"] = new SelectList(_context.pets, "name", "name");
            ViewData["keeper_ID"] = new SelectList(_context.keepers, "ID", "name");
            return View();
        }
    }
}
