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
    public class wardsController : Controller
    {
        private readonly TailstaleContext _context;

        public wardsController(TailstaleContext context)
        {
            _context = context;
        }

        // GET: wards
        public async Task<IActionResult> Index()
        {
            var tailstaleContext = _context.wards.Include(w => w.business);
            return View(await tailstaleContext.ToListAsync());
        }

        // GET: wards/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ward = await _context.wards
                .Include(w => w.business)
                .FirstOrDefaultAsync(m => m.ward_ID == id);
            if (ward == null)
            {
                return NotFound();
            }

            return View(ward);
        }

        // GET: wards/Create
        public IActionResult Create()
        {
            ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name");
            return View();
        }

        // POST: wards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ward_ID,business_ID,status,memo")] ward ward)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ward);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", ward.business_ID);
            return View(ward);
        }

        // GET: wards/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ward = await _context.wards.FindAsync(id);
            if (ward == null)
            {
                return NotFound();
            }
            ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", ward.business_ID);
            return View(ward);
        }

        // POST: wards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ward_ID,business_ID,status,memo")] ward ward)
        {
            if (id != ward.ward_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ward);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!wardExists(ward.ward_ID))
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
            ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", ward.business_ID);
            return View(ward);
        }

        // GET: wards/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ward = await _context.wards
                .Include(w => w.business)
                .FirstOrDefaultAsync(m => m.ward_ID == id);
            if (ward == null)
            {
                return NotFound();
            }

            return View(ward);
        }

        // POST: wards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ward = await _context.wards.FindAsync(id);
            if (ward != null)
            {
                _context.wards.Remove(ward);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool wardExists(int id)
        {
            return _context.wards.Any(e => e.ward_ID == id);
        }
    }
}
