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
    public class medical_orderController : Controller
    {
        private readonly TailstaleContext _context;

        public medical_orderController(TailstaleContext context)
        {
            _context = context;
        }

        // GET: medical_order
        public async Task<IActionResult> Index()
        {
            var tailstaleContext = _context.medical_orders.Include(m => m.medical_records);
            return View(await tailstaleContext.ToListAsync());
        }

        // GET: medical_order/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medical_order = await _context.medical_orders
                .Include(m => m.medical_records)
                .FirstOrDefaultAsync(m => m.id == id);
            if (medical_order == null)
            {
                return NotFound();
            }

            return View(medical_order);
        }

        // GET: medical_order/Create
        public IActionResult Create()
        {
            ViewData["medical_records_id"] = new SelectList(_context.medical_records, "id", "complain");
            return View();
        }

        // POST: medical_order/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,datetime,medical_records_id")] medical_order medical_order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(medical_order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["medical_records_id"] = new SelectList(_context.medical_records, "id", "complain", medical_order.medical_records_id);
            return View(medical_order);
        }

        // GET: medical_order/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medical_order = await _context.medical_orders.FindAsync(id);
            if (medical_order == null)
            {
                return NotFound();
            }
            ViewData["medical_records_id"] = new SelectList(_context.medical_records, "id", "complain", medical_order.medical_records_id);
            return View(medical_order);
        }

        // POST: medical_order/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,datetime,medical_records_id")] medical_order medical_order)
        {
            if (id != medical_order.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medical_order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!medical_orderExists(medical_order.id))
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
            ViewData["medical_records_id"] = new SelectList(_context.medical_records, "id", "complain", medical_order.medical_records_id);
            return View(medical_order);
        }

        // GET: medical_order/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medical_order = await _context.medical_orders
                .Include(m => m.medical_records)
                .FirstOrDefaultAsync(m => m.id == id);
            if (medical_order == null)
            {
                return NotFound();
            }

            return View(medical_order);
        }

        // POST: medical_order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medical_order = await _context.medical_orders.FindAsync(id);
            if (medical_order != null)
            {
                _context.medical_orders.Remove(medical_order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool medical_orderExists(int id)
        {
            return _context.medical_orders.Any(e => e.id == id);
        }
    }
}
