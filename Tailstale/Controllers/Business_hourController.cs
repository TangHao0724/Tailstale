using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tailstale.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace Tailstale.Controllers
{
    public class Business_hourController : Controller
    {
        private readonly TailstaleContext _context;

        public Business_hourController(TailstaleContext context)
        {
            _context = context;
        }

        //GET: Business_hour 
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string business_day)
        {
            //var tailstaleContext = _context.Business_hours.Include(b => b.business);
            //return View(await tailstaleContext.ToListAsync());
            DateOnly selectedDate = DateOnly.Parse(business_day);

            // 查詢符合條件的 Business_hour 記錄
            var businessHours = _context.Business_hour
                .Include(bh => bh.business)
                .Where(bh => bh.business_day == selectedDate)
                .ToList();

            // 返回到前端，假設你的 View 期望接收一段 HTML 作為結果
            return PartialView("_hourPartial", businessHours);
        }


        //public async Task<IActionResult> Orders(DateOnly? selectedDate)//對orders右鍵新增檢視,選razor檢視>,名稱用_OrdersPartial,範本選List(客戶可能很多訂單),模型類別選order(訂單),db選Nort...,打勾局部檢視
        //{
        //    var query = _context.Business_hour.AsQueryable();


        //    if (selectedDate.HasValue)
        //    {
        //        // 篩選選定日期的資料
        //        query = query.Where(bh => bh.business_day == selectedDate.Value);
        //    }

        //    var businessHours = await query.ToListAsync();

        //    return View(businessHours);
        //}




        // GET: Business_hour/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var business_hour = await _context.Business_hour
                .Include(b => b.business)
                .FirstOrDefaultAsync(m => m.id == id);
            if (business_hour == null)
            {
                return NotFound();
            }

            return View(business_hour);
        }

        // GET: Business_hour/Create
        public IActionResult Create()
        {
            ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name");
            return View();
        }

        // POST: Business_hour/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,business_ID,business_day,people_limit")] Business_hour business_hour)
        {
            if (ModelState.IsValid)
            {
                _context.Add(business_hour);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", business_hour.business_ID);
            return View(business_hour);
        }

        // GET: Business_hour/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var business_hour = await _context.Business_hour.FindAsync(id);
            if (business_hour == null)
            {
                return NotFound();
            }
            ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", business_hour.business_ID);
            return View(business_hour);
        }

        // POST: Business_hour/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,business_ID,business_day,people_limit")] Business_hour business_hour)
        {
            if (id != business_hour.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(business_hour);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Business_hourExists(business_hour.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", business_hour.business_ID);
            return View(business_hour);
        }

        // GET: Business_hour/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var business_hour = await _context.Business_hour
                .Include(b => b.business)
                .FirstOrDefaultAsync(m => m.id == id);
            if (business_hour == null)
            {
                return NotFound();
            }

            return View(business_hour);
        }

        // POST: Business_hour/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var business_hour = await _context.Business_hour.FindAsync(id);
            if (business_hour != null)
            {
                _context.Business_hour.Remove(business_hour);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Business_hourExists(int id)
        {
            return _context.Business_hour.Any(e => e.id == id);
        }
    }
}
