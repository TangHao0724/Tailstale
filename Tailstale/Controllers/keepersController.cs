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
    public class keepersController : Controller
    {
        private readonly TailstaleContext _context;

        public keepersController(TailstaleContext context)
        {
            _context = context;
        }

        // GET: keepers
        public async Task<IActionResult> Index()
        {
            var tailstaleContext = _context.keepers.Include(k => k.statusNavigation);
            return View(await tailstaleContext.ToListAsync());
        }

        // GET: keepers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var keeper = await _context.keepers
                .Include(k => k.statusNavigation)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (keeper == null)
            {
                return NotFound();
            }

            return View(keeper);
        }

        // GET: keepers/Create
        public IActionResult Create()
        {
            ViewData["status"] = new SelectList(_context.member_statuses, "ID", "status_name");
            return View();
        }

        // POST: keepers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,password,name,phone,email,address,status,created_at")] keeper keeper)
        {
            if (ModelState.IsValid)
            {
                _context.Add(keeper);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["status"] = new SelectList(_context.member_statuses, "ID", "status_name", keeper.status);
            return View(keeper);
        }

        // GET: keepers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var keeper = await _context.keepers.FindAsync(id);
            if (keeper == null)
            {
                return NotFound();
            }
            ViewData["status"] = new SelectList(_context.member_statuses, "ID", "status_name", keeper.status);
            return View(keeper);
        }

        // POST: keepers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,password,name,phone,email,address,status,created_at")] keeper keeper)
        {
            if (id != keeper.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(keeper);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!keeperExists(keeper.ID))
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
            ViewData["status"] = new SelectList(_context.member_statuses, "ID", "status_name", keeper.status);
            return View(keeper);
        }

        // GET: keepers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var keeper = await _context.keepers
                .Include(k => k.statusNavigation)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (keeper == null)
            {
                return NotFound();
            }

            return View(keeper);
        }

        // POST: keepers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var keeper = await _context.keepers.FindAsync(id);
            if (keeper != null)
            {
                _context.keepers.Remove(keeper);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool keeperExists(int id)
        {
            return _context.keepers.Any(e => e.ID == id);
        }
    }
}
