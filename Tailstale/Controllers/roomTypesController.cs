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
    public class roomTypesController : Controller
    {
        private readonly TailstaleContext _context;

        public roomTypesController(TailstaleContext context)
        {
            _context = context;
        }

        // GET: roomTypes
        public async Task<IActionResult> Index()
        {
            var hotelID = (int)(HttpContext.Session.GetInt32("hotelID11"));
            var tailstaleContext = _context.roomTypes.Include(r => r.FK_business).Where(r=>r.FK_businessID==hotelID);
            return View(await tailstaleContext.ToListAsync());
        }

        // GET: roomTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roomType = await _context.roomTypes
                .Include(r => r.FK_business)
                .FirstOrDefaultAsync(m => m.roomType_ID == id);
            if (roomType == null)
            {
                return NotFound();
            }

            return View(roomType);
        }

        // GET: roomTypes/Create
        public IActionResult Create()
        {
            ViewData["FK_businessID"] = new SelectList(_context.businesses, "ID", "name");
            return View();
        }

        // POST: roomTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("roomType_ID,roomType1,FK_businessID")] roomType roomType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(roomType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FK_businessID"] = new SelectList(_context.businesses, "ID", "name", roomType.FK_businessID);
            return View(roomType);
        }

        // GET: roomTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roomType = await _context.roomTypes.FindAsync(id);
            if (roomType == null)
            {
                return NotFound();
            }
            ViewData["FK_businessID"] = new SelectList(_context.businesses, "ID", "name", roomType.FK_businessID);
            return View(roomType);
        }

        // POST: roomTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("roomType_ID,roomType1,FK_businessID")] roomType roomType)
        {
            if (id != roomType.roomType_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(roomType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!roomTypeExists(roomType.roomType_ID))
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
            ViewData["FK_businessID"] = new SelectList(_context.businesses, "ID", "name", roomType.FK_businessID);
            return View(roomType);
        }

        // GET: roomTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roomType = await _context.roomTypes
                .Include(r => r.FK_business)
                .FirstOrDefaultAsync(m => m.roomType_ID == id);
            if (roomType == null)
            {
                return NotFound();
            }

            return View(roomType);
        }

        // POST: roomTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var roomType = await _context.roomTypes.FindAsync(id);
            if (roomType != null)
            {
                _context.roomTypes.Remove(roomType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool roomTypeExists(int id)
        {
            return _context.roomTypes.Any(e => e.roomType_ID == id);
        }
    }
}
