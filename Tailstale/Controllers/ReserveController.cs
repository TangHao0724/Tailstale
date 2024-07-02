using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tailstale.Models;

namespace Tailstale.Controllers
{
    public class ReserveController : Controller
    {
        private readonly TailstaleContext _context;

        public ReserveController(TailstaleContext context)
        {
            _context = context;
        }

        // GET: Reserves
        public async Task<IActionResult> Index()
        {
            //var tailstaleContext = _context.Reserve.Include(r => r.business).Include(r => r.keeper);
            //return View(await tailstaleContext.ToListAsync());
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string time)
        {
            //var tailstaleContext = _context.Business_hours.Include(b => b.business);
            //return View(await tailstaleContext.ToListAsync());
            if (string.IsNullOrEmpty(time))
            {
                return PartialView("_ReservehourPartial", new List<Reserve>());
            }

            DateTime selectedDate;
            if (!DateTime.TryParse(time, out selectedDate))
            {
                // 如果转换失败，则返回空的 PartialView
                return PartialView("_ReservehourPartial", new List<Reserve>());
            }

            //DateTime selectedDate = DateTime.Parse(time);

            // 查詢符合條件的 Business_hour 記錄
            var businessHours = _context.Reserve
                .Include(bh => bh.business)//這段讓view的item.business.name可以用id對應到name並顯示,當然前提這兩個表要有導覽屬性(外來建)
                .Include(bh => bh.keeper)
                .Where(bh => bh.time.Date == selectedDate)
                .ToList();

            // 返回到前端，假設你的 View 期望接收一段 HTML 作為結果
            return PartialView("_ReservehourPartial", businessHours);
        }




        // GET: Reserves/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserve = await _context.Reserve
                .Include(r => r.business)
                .Include(r => r.keeper)
                .FirstOrDefaultAsync(m => m.id == id);
            if (reserve == null)
            {
                return NotFound();
            }

            return View(reserve);
        }

        // GET: Reserves/Create
        public IActionResult Create()
        {
            ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name");
            ViewData["keeper_id"] = new SelectList(_context.keepers, "ID", "name");
            return View();
        }

        // POST: Reserves/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,keeper_id,pet_name,business_ID,time,service_name,created_at")] Reserve reserve)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reserve);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", reserve.business_ID);
            ViewData["keeper_id"] = new SelectList(_context.keepers, "ID", "address", reserve.keeper_id);
            return View(reserve);
        }

        // GET: Reserves/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserve = await _context.Reserve.FindAsync(id);
            if (reserve == null)
            {
                return NotFound();
            }
            ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", reserve.business_ID);
            ViewData["keeper_id"] = new SelectList(_context.keepers, "ID", "address", reserve.keeper_id);
            return View(reserve);
        }

        // POST: Reserves/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,keeper_id,pet_name,business_ID,time,service_name,created_at")] Reserve reserve)
        {
            if (id != reserve.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reserve);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReserveExists(reserve.id))
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
            ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", reserve.business_ID);
            ViewData["keeper_id"] = new SelectList(_context.keepers, "ID", "address", reserve.keeper_id);
            return View(reserve);
        }

        // GET: Reserves/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserve = await _context.Reserve
                .Include(r => r.business)
                .Include(r => r.keeper)
                .FirstOrDefaultAsync(m => m.id == id);
            if (reserve == null)
            {
                return NotFound();
            }

            return View(reserve);
        }

        // POST: Reserves/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reserve = await _context.Reserve.FindAsync(id);
            if (reserve != null)
            {
                _context.Reserve.Remove(reserve);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReserveExists(int id)
        {
            return _context.Reserve.Any(e => e.id == id);
        }
    }
}
