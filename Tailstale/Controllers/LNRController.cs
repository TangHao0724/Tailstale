using CRUD_COREMVC;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Tailstale.Filter;
using Tailstale.Models;

namespace Tailstale.Controllers
{
    [IsLoginFilter]
    public class LNRController : Controller
    {
        private readonly TailstaleContext _context;

        public LNRController(TailstaleContext context)
        {
            _context = context;
        }

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
        [IsHotelFilter]
        public async Task<IActionResult> HotelIndex()
        {
            ViewBag.loginID = HttpContext.Session.GetInt32("loginID");
            ViewBag.loginType = HttpContext.Session.GetInt32("loginType");
            ViewBag.loginName = await _context.businesses
                                .Where(b => b.ID == HttpContext.Session.GetInt32("loginID"))
                                .Select(b => b.name)
                                .FirstOrDefaultAsync();

            return View();
        }
        [IsSalonFilter]
        public async Task<IActionResult> SalonIndex()
        {
            ViewBag.loginID = HttpContext.Session.GetInt32("loginID");
            ViewBag.loginType = HttpContext.Session.GetInt32("loginType");
            ViewBag.loginName = await _context.businesses
                    .Where(b => b.ID == HttpContext.Session.GetInt32("loginID"))
                    .Select(b => b.name)
                    .FirstOrDefaultAsync();
            return View();
        }
        [IsHospitalFilter]
        public async Task<IActionResult> HospitalIndex()
        {
            
            ViewBag.loginID = HttpContext.Session.GetInt32("loginID");
            ViewBag.loginType = HttpContext.Session.GetInt32("loginType");
            ViewBag.loginName = await _context.businesses
                                .Where(b => b.ID == HttpContext.Session.GetInt32("loginID"))
                                .Select(b => b.name)
                                .FirstOrDefaultAsync();
            return View();
        }
    }
}
