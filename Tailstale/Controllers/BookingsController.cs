using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tailstale.Models;

namespace Tailstale.Controllers
{
    public class BookingsController : Controller
    {
        private readonly TailstaleContext _context;

        public BookingsController(TailstaleContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            var tailstaleContext = _context.Bookings.Include(b => b.hotel).Include(b => b.keeper);
            return View(await tailstaleContext.ToListAsync());
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.hotel)
                .Include(b => b.keeper)
                .FirstOrDefaultAsync(m => m.bookingID == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewData["hotelID"] = new SelectList(_context.businesses, "ID", "name");
            ViewData["keeper_ID"] = new SelectList(_context.keepers, "ID", "address");
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [DisplayName("新增")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("bookingID,keeper_ID,hotelID,checkinDate,checkoutDate,bookingAmountTotal,bookingStatus,bookingDate")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["hotelID"] = new SelectList(_context.businesses, "ID", "name", booking.hotelID);
            ViewData["keeper_ID"] = new SelectList(_context.keepers, "ID", "address", booking.keeper_ID);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["hotelID"] = new SelectList(_context.businesses, "ID", "name", booking.hotelID);
            ViewData["keeper_ID"] = new SelectList(_context.keepers, "ID", "address", booking.keeper_ID);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("bookingID,keeper_ID,hotelID,checkinDate,checkoutDate,bookingAmountTotal,bookingStatus,bookingDate")] Booking booking)
        {
            if (id != booking.bookingID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.bookingID))
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
            ViewData["hotelID"] = new SelectList(_context.businesses, "ID", "name", booking.hotelID);
            ViewData["keeper_ID"] = new SelectList(_context.keepers, "ID", "address", booking.keeper_ID);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.hotel)
                .Include(b => b.keeper)
                .FirstOrDefaultAsync(m => m.bookingID == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.bookingID == id);
        }
    }
}
