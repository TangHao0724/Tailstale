using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tailstale.Models;
using Tailstale.Index_ViewModel;
using System.Drawing;

namespace Tailstale.Controllers
{

    public class UserMangerController : Controller
    {
        private readonly TailstaleContext _context;
        public UserMangerController(TailstaleContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var tailstaleContext = _context.keepers.Include(k => k.statusNavigation);
            return View(await tailstaleContext.ToListAsync());
        }

    }
}
