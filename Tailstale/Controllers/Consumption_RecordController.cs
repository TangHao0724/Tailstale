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
    public class Consumption_RecordController : Controller
    {
        private readonly TailstaleContext _context;

        public Consumption_RecordController(TailstaleContext context)
        {
            _context = context;
        }

        // GET: Consumption_Record
        public async Task<IActionResult> Index()
        {
            var tailstaleContext = _context.Consumption_Records.Include(c => c.beautician).Include(c => c.business).Include(c => c.keeper);
            return View(await tailstaleContext.ToListAsync());
        }

        // GET: Consumption_Record/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consumption_Record = await _context.Consumption_Records
                .Include(c => c.beautician)
                .Include(c => c.business)
                .Include(c => c.keeper)
                .FirstOrDefaultAsync(m => m.id == id);
            if (consumption_Record == null)
            {
                return NotFound();
            }

            return View(consumption_Record);
        }

        // GET: Consumption_Record/Create
        public IActionResult Create()
        {
            ViewData["beautician_id"] = new SelectList(_context.Beautician, "id", "gender");
            ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name");
            ViewData["keeper_id"] = new SelectList(_context.keepers, "ID", "address");
            return View();
        }

        // POST: Consumption_Record/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,keeper_id,pet_name,business_ID,time,beautician_id,service_name,pet_weight,price,end_time,before_photo,after_photo")] Consumption_Record consumption_Record)
        {
            if (ModelState.IsValid)
            {
                _context.Add(consumption_Record);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["beautician_id"] = new SelectList(_context.Beautician, "id", "gender", consumption_Record.beautician_id);
            ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", consumption_Record.business_ID);
            ViewData["keeper_id"] = new SelectList(_context.keepers, "ID", "address", consumption_Record.keeper_id);
            return View(consumption_Record);
        }

        // GET: Consumption_Record/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consumption_Record = await _context.Consumption_Records.FindAsync(id);
            if (consumption_Record == null)
            {
                return NotFound();
            }
            ViewData["beautician_id"] = new SelectList(_context.Beautician, "id", "gender", consumption_Record.beautician_id);
            ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", consumption_Record.business_ID);
            ViewData["keeper_id"] = new SelectList(_context.keepers, "ID", "address", consumption_Record.keeper_id);
            return View(consumption_Record);
        }

        // POST: Consumption_Record/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,keeper_id,pet_name,business_ID,time,beautician_id,service_name,pet_weight,price,end_time,before_photo,after_photo")] Consumption_Record consumption_Record)
        {
            if (id != consumption_Record.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(consumption_Record);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Consumption_RecordExists(consumption_Record.id))
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
            ViewData["beautician_id"] = new SelectList(_context.Beautician, "id", "gender", consumption_Record.beautician_id);
            ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", consumption_Record.business_ID);
            ViewData["keeper_id"] = new SelectList(_context.keepers, "ID", "address", consumption_Record.keeper_id);
            return View(consumption_Record);
        }

        // GET: Consumption_Record/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consumption_Record = await _context.Consumption_Records
                .Include(c => c.beautician)
                .Include(c => c.business)
                .Include(c => c.keeper)
                .FirstOrDefaultAsync(m => m.id == id);
            if (consumption_Record == null)
            {
                return NotFound();
            }

            return View(consumption_Record);
        }

        // POST: Consumption_Record/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var consumption_Record = await _context.Consumption_Records.FindAsync(id);
            if (consumption_Record != null)
            {
                _context.Consumption_Records.Remove(consumption_Record);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Consumption_RecordExists(int id)
        {
            return _context.Consumption_Records.Any(e => e.id == id);
        }
    }
}
