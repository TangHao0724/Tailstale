using CRUD_COREMVC;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Drawing;
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
            int? KloginID = HttpContext.Session.GetInt32("KloginID");
            int? loginID = HttpContext.Session.GetInt32("loginID");
            int? loginType = HttpContext.Session.GetInt32("loginType");

            string KloginName = HttpContext.Session.GetString("KloginName");
            

          


            // 將值設置到 ViewBag
            ViewBag.LoginName = KloginName;


            ViewBag.KLoginID = KloginID.HasValue ? KloginID.Value : (int?)null;
            ViewBag.LoginID = loginID.HasValue ? loginID.Value : (int?)null;
            ViewBag.LoginType = loginType.HasValue ? loginType.Value : (int?)null;
            ViewData["keeper_ID"] = new SelectList(_context.keepers, "ID", "name");

            if (KloginID.HasValue)
            {
                // loginID 有值，進行查詢
                var pets = _context.pets
                                   .Where(p => p.keeper_ID == KloginID.Value)
                                   .ToList();

                if (pets.Any())
                {
                    // 找到匹配的資料

                    ViewData["pet_name"] = new SelectList(pets, "name", "name");
                }
                else
                {
                    //ViewData["pet_name"] = new SelectList(Enumerable.Empty<SelectListItem>());//可有可無
                }
            }


            return View();
        }
    }
}
