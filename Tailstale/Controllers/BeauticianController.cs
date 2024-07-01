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
    public class BeauticianController : Controller
    {
        private readonly TailstaleContext _context;

        public BeauticianController(TailstaleContext context)
        {
            _context = context;
        }

        // GET: Beautician
        public async Task<IActionResult> Index()
        {
            var tailstaleContext = _context.Beautician.Include(b => b.business);
            return View(await tailstaleContext.ToListAsync());
        }

        // GET: Beautician/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var beautician = await _context.Beautician
                .Include(b => b.business)
                .FirstOrDefaultAsync(m => m.id == id);
            if (beautician == null)
            {
                return NotFound();
            }

            return View(beautician);
        }

        // GET: Beautician/Create
        public IActionResult Create()
        {
            ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name");
            return View();
        }

        // POST: Beautician/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,name,gender,photo,phone,business_ID,Highest_license,Remark")] Beautician beautician)
        {
            if (ModelState.IsValid)
            {
                _context.Add(beautician);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", beautician.business_ID);
            return View(beautician);
        }

        // GET: Beautician/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var beautician = await _context.Beautician.FindAsync(id);
            if (beautician == null)
            {
                return NotFound();
            }
            ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", beautician.business_ID);
            return View(beautician);
        }

        // POST: Beautician/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,name,gender,photo,phone,business_ID,Highest_license,Remark")] Beautician beautician)
        {
            if (id != beautician.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(beautician);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BeauticianExists(beautician.id))
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
            ViewData["business_ID"] = new SelectList(_context.businesses, "ID", "name", beautician.business_ID);
            return View(beautician);
        }

        // GET: Beautician/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var beautician = await _context.Beautician
                .Include(b => b.business)
                .FirstOrDefaultAsync(m => m.id == id);
            if (beautician == null)
            {
                return NotFound();
            }

            return View(beautician);
        }

        // POST: Beautician/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var beautician = await _context.Beautician.FindAsync(id);
            if (beautician != null)
            {
                _context.Beautician.Remove(beautician);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BeauticianExists(int id)
        {
            return _context.Beautician.Any(e => e.id == id);
        }
    }
}
