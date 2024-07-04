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
    public class HospController : Controller
    {
        private readonly TailstaleContext _context;

        public HospController(TailstaleContext context)
        {
            _context = context;
        }

        // GET: Hosp
        public async Task<IActionResult> Index()
        {
            var tailstaleContext = _context.hosp_histories.Include(h => h.medical_record).Include(h => h.nursing_record).Include(h => h.ward);
            return View(await tailstaleContext.ToListAsync());
        }

        // GET: Hosp/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hosp_history = await _context.hosp_histories
                .Include(h => h.medical_record)
                .Include(h => h.nursing_record)
                .Include(h => h.ward)
                .FirstOrDefaultAsync(m => m.id == id);
            if (hosp_history == null)
            {
                return NotFound();
            }

            return View(hosp_history);
        }

        // GET: Hosp/Create
        public IActionResult Create()
        {
            ViewData["medical_record_id"] = new SelectList(_context.medical_records, "id", "admission_process");
            ViewData["nursing_record_id"] = new SelectList(_context.nursing_records, "id", "id");
            ViewData["ward_id"] = new SelectList(_context.wards, "ward_ID", "ward_ID");
            return View();
        }

        // POST: Hosp/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,medical_record_id,admission_date,discharge_date,nursing_record_id,ward_id,memo")] hosp_history hosp_history)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hosp_history);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["medical_record_id"] = new SelectList(_context.medical_records, "id", "admission_process", hosp_history.medical_record_id);
            ViewData["nursing_record_id"] = new SelectList(_context.nursing_records, "id", "id", hosp_history.nursing_record_id);
            ViewData["ward_id"] = new SelectList(_context.wards, "ward_ID", "ward_ID", hosp_history.ward_id);
            return View(hosp_history);
        }

        // GET: Hosp/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hosp_history = await _context.hosp_histories.FindAsync(id);
            if (hosp_history == null)
            {
                return NotFound();
            }
            ViewData["medical_record_id"] = new SelectList(_context.medical_records, "id", "admission_process", hosp_history.medical_record_id);
            ViewData["nursing_record_id"] = new SelectList(_context.nursing_records, "id", "id", hosp_history.nursing_record_id);
            ViewData["ward_id"] = new SelectList(_context.wards, "ward_ID", "ward_ID", hosp_history.ward_id);
            return View(hosp_history);
        }

        // POST: Hosp/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,medical_record_id,admission_date,discharge_date,nursing_record_id,ward_id,memo")] hosp_history hosp_history)
        {
            if (id != hosp_history.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hosp_history);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!hosp_historyExists(hosp_history.id))
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
            ViewData["medical_record_id"] = new SelectList(_context.medical_records, "id", "admission_process", hosp_history.medical_record_id);
            ViewData["nursing_record_id"] = new SelectList(_context.nursing_records, "id", "id", hosp_history.nursing_record_id);
            ViewData["ward_id"] = new SelectList(_context.wards, "ward_ID", "ward_ID", hosp_history.ward_id);
            return View(hosp_history);
        }

        // GET: Hosp/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hosp_history = await _context.hosp_histories
                .Include(h => h.medical_record)
                .Include(h => h.nursing_record)
                .Include(h => h.ward)
                .FirstOrDefaultAsync(m => m.id == id);
            if (hosp_history == null)
            {
                return NotFound();
            }

            return View(hosp_history);
        }

        // POST: Hosp/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hosp_history = await _context.hosp_histories.FindAsync(id);
            if (hosp_history != null)
            {
                _context.hosp_histories.Remove(hosp_history);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool hosp_historyExists(int id)
        {
            return _context.hosp_histories.Any(e => e.id == id);
        }
    }
}
