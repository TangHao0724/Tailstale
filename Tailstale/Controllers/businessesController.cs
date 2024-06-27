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
    public class businessesController : Controller
    {
        private readonly TailstaleContext _context;

        public businessesController(TailstaleContext context)
        {
            _context = context;
        }

        // GET: businesses
        public async Task<IActionResult> Index()
        {
            var tailstaleContext = _context.businesses.Include(b => b.FK_status).Include(b => b.type);
            return View(await tailstaleContext.ToListAsync());
        }

        // GET: businesses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var business = await _context.businesses
                .Include(b => b.FK_status)
                .Include(b => b.type)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (business == null)
            {
                return NotFound();
            }

            return View(business);
        }

        // GET: businesses/Create
        public IActionResult Create()
        {
            ViewData["FK_status_ID"] = new SelectList(_context.statuses, "ID", "status_name");
            ViewData["type_ID"] = new SelectList(_context.business_types, "business_type_ID", "business_type_name");
            return View();
        }

        // POST: businesses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,password,type_ID,name,email,phone,address,geoJson,license_number,FK_status_ID,photo_url,created_at")] business business)
        {
            if (ModelState.IsValid)
            {
                _context.Add(business);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FK_status_ID"] = new SelectList(_context.statuses, "ID", "status_name", business.FK_status_ID);
            ViewData["type_ID"] = new SelectList(_context.business_types, "business_type_ID", "business_type_name", business.type_ID);
            return View(business);
        }

        // GET: businesses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var business = await _context.businesses.FindAsync(id);
            if (business == null)
            {
                return NotFound();
            }
            ViewData["FK_status_ID"] = new SelectList(_context.statuses, "ID", "status_name", business.FK_status_ID);
            ViewData["type_ID"] = new SelectList(_context.business_types, "business_type_ID", "business_type_name", business.type_ID);
            return View(business);
        }

        // POST: businesses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,password,type_ID,name,email,phone,address,geoJson,license_number,FK_status_ID,photo_url,created_at")] business business)
        {
            if (id != business.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(business);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!businessExists(business.ID))
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
            ViewData["FK_status_ID"] = new SelectList(_context.statuses, "ID", "status_name", business.FK_status_ID);
            ViewData["type_ID"] = new SelectList(_context.business_types, "business_type_ID", "business_type_name", business.type_ID);
            return View(business);
        }

        // GET: businesses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var business = await _context.businesses
                .Include(b => b.FK_status)
                .Include(b => b.type)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (business == null)
            {
                return NotFound();
            }

            return View(business);
        }

        // POST: businesses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var business = await _context.businesses.FindAsync(id);
            if (business != null)
            {
                _context.businesses.Remove(business);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool businessExists(int id)
        {
            return _context.businesses.Any(e => e.ID == id);
        }
    }
}
