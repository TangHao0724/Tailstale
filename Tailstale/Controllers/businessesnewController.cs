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
    public class businessesnewController : Controller
    {
        private readonly TailstaleContext _context;

        public businessesnewController(TailstaleContext context)
        {
            _context = context;
        }

        // GET: businessesnew
        public async Task<IActionResult> Index()
        {
            var tailstaleContext = _context.businesses.Include(b => b.business_statusNavigation).Include(b => b.type);
            return View(await tailstaleContext.ToListAsync());
        }

        // GET: businessesnew/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var business = await _context.businesses
                .Include(b => b.business_statusNavigation)
                .Include(b => b.type)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (business == null)
            {
                return NotFound();
            }

            return View(business);
        }

        // GET: businessesnew/Create
        public IActionResult Create()
        {
            ViewData["business_status"] = new SelectList(_context.member_statuses, "member_status_ID", "status_name");
            ViewData["type_ID"] = new SelectList(_context.business_types, "business_type_ID", "business_type_name");
            return View();
        }

        // POST: businessesnew/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,password,salt,type_ID,name,email,phone,address,geoJson,license_number,business_status,photo_url,created_at")] business business)
        {
            if (ModelState.IsValid)
            {
                _context.Add(business);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["business_status"] = new SelectList(_context.member_statuses, "member_status_ID", "status_name", business.business_status);
            ViewData["type_ID"] = new SelectList(_context.business_types, "business_type_ID", "business_type_name", business.type_ID);
            return View(business);
        }

        // GET: businessesnew/Edit/5
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
            ViewData["business_status"] = new SelectList(_context.member_statuses, "member_status_ID", "status_name", business.business_status);
            ViewData["type_ID"] = new SelectList(_context.business_types, "business_type_ID", "business_type_name", business.type_ID);
            return View(business);
        }

        // POST: businessesnew/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,password,salt,type_ID,name,email,phone,address,geoJson,license_number,business_status,photo_url,created_at")] business business)
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
            ViewData["business_status"] = new SelectList(_context.member_statuses, "member_status_ID", "status_name", business.business_status);
            ViewData["type_ID"] = new SelectList(_context.business_types, "business_type_ID", "business_type_name", business.type_ID);
            return View(business);
        }

        // GET: businessesnew/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var business = await _context.businesses
                .Include(b => b.business_statusNavigation)
                .Include(b => b.type)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (business == null)
            {
                return NotFound();
            }

            return View(business);
        }

        // POST: businessesnew/Delete/5
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
