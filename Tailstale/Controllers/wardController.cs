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
    public class wardController : Controller
    {
        private readonly TailstaleContext _context;

        public wardController(TailstaleContext context)
        {
            _context = context;
        }

        // GET: ward
        public async Task<IActionResult> Index()
        {
            var tailstaleContext = _context.wards.Include(w => w.business).OrderBy(w=>w.ward_status);
            return View(tailstaleContext);
        }

        // GET: ward/Details/5
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

        // GET: ward/Create
        public IActionResult Create()
        {
            ViewData["business_ID"] = new SelectList(_context.businesses.Where(b=>b.type_ID==3), "ID", "name");
            return View();
        }

        // POST: ward/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ward_ID,ward_name,business_ID,ward_status,memo")] ward ward)
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

        // GET: ward/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var ward = await _context.wards.FindAsync(id);
        //    if (ward == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", ward.business_ID);
        //    return View(ward);
        //}

        // POST: ward/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ward_ID,ward_name,business_ID,ward_status,memo")] ward ward)
        {
            if (ward.ward_ID == 0)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var current_ward = _context.wards.Find(ward.ward_ID);

                current_ward.ward_ID = ward.ward_ID;
                current_ward.ward_name = ward.ward_name;
                current_ward.business_ID = ward.business_ID;
                current_ward.ward_status = ward.ward_status;
                current_ward.memo = ward.memo;

                try
                {
                    _context.Update(current_ward);
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
            return NotFound();
        }
        // GET: ward/Delete/5
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

        // POST: ward/Delete/5
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
