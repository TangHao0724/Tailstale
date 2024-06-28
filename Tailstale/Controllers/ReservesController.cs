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
    public class ReservesController : Controller
    {
        private readonly TailstaleContext _context;

        public ReservesController(TailstaleContext context)
        {
            _context = context;
        }

        // GET: Reserves
        public async Task<IActionResult> Index()
        {
            var tailstaleContext = _context.Reserves.Include(r => r.business).Include(r => r.keeper);
            return View(await tailstaleContext.ToListAsync());
        }

        // GET: Reserves/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserve = await _context.Reserves
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
            ViewData["keeper_id"] = new SelectList(_context.keepers, "ID", "address");
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

            var reserve = await _context.Reserves.FindAsync(id);
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

            var reserve = await _context.Reserves
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
            var reserve = await _context.Reserves.FindAsync(id);
            if (reserve != null)
            {
                _context.Reserves.Remove(reserve);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReserveExists(int id)
        {
            return _context.Reserves.Any(e => e.id == id);
        }
    }
}
