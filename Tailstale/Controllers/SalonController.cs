using CRUD_COREMVC;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

            var addresses = _context.businesses
           .Where(b => b.type_ID == 2 && b.address.Length >= 4)
           .Select(b => b.address)
           .ToList();

            ViewBag.Addresses = addresses;



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


        //public async Task<IActionResult> KeeperEdit()
        //{

        //    int? KloginID = HttpContext.Session.GetInt32("KloginID");
        //    ViewBag.KLoginID = KloginID.HasValue ? KloginID.Value : (int?)null;


        //    if (KloginID == null)
        //    {
        //        return NotFound();
        //    }

        //    var reserve = await _context.Reserves.FindAsync(KloginID);
        //    if (reserve == null)
        //    {
        //        return NotFound();
        //    }


            

           
        //    return View(reserve);
        //}

        //// POST: Reserves/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> KeeperEdit(int id, [Bind("id,keeper_id,pet_name,business_ID,time,service_name,created_at,status")] Reserve reserve)
        //{
        //    if (id != reserve.id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(reserve);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ReserveExists(reserve.id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }

        //    var pets = _context.pets
        //    .Where(p => p.keeper_ID == reserve.keeper_id) // 根据 keeper_id 进行过滤
        //   .ToList();
        //    var orderstatus = _context.order_statuses
        //       .Where(b => b.business_type_ID == 2)
        //       .ToList();

        //    int? loginID = HttpContext.Session.GetInt32("loginID");
        //    var businesses = _context.businesses
        //   .Where(b => b.ID == loginID)
        //   .ToList();

        //    var keeper = _context.keepers
        //   .Where(p => p.ID == reserve.keeper_id) // 根据 keeper_id 进行过滤
        //  .ToList();

        //    ViewData["business_ID"] = new SelectList(businesses, "ID", "name");
        //    ViewData["Orderstatus_ID"] = new SelectList(orderstatus, "ID", "status_name", reserve.status);
        //    ViewData["pet_name"] = new SelectList(pets, "name", "name");
        //    ViewData["keeper_id"] = new SelectList(keeper, "ID", "name");
        //    //ViewData["keeper_id"] = new SelectList(_context.keepers, "ID", "address", reserve.keeper_id);
        //    return View(reserve);
        //}






    }
}
