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
    public class BookingDetailsController : Controller
    {
        private readonly TailstaleContext _context;

        public BookingDetailsController(TailstaleContext context)
        {
            _context = context;
        }

        // GET: BookingDetails
        public async Task<IActionResult> Index()
        {
            var tailstaleContext = _context.BookingDetails.Include(b => b.booking).Include(b => b.room);
            return View(await tailstaleContext.ToListAsync());
        }

        // GET: BookingDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookingDetail = await _context.BookingDetails
                .Include(b => b.booking)
                .Include(b => b.room)
                .FirstOrDefaultAsync(m => m.bdID == id);
            if (bookingDetail == null)
            {
                return NotFound();
            }

            return View(bookingDetail);
        }

        // GET: BookingDetails/Create
        public IActionResult Create()
        {
            ViewData["bookingID"] = new SelectList(_context.Bookings, "bookingID", "bookingID");
            ViewData["roomID"] = new SelectList(_context.Rooms, "roomID", "roomID");

           
            return View();
        }
        //POST:BookingDetails/GetRoomPrice
        public async Task<int?> GetRoomPrice(int roomID)
        {
            // 根據 roomID 從資料庫或其他數據源中查詢房間價格
          
        
            int? rc = _context.Rooms.Where(c => c.roomID == roomID).Select(r => r.roomPrice).FirstOrDefault(); ;
           

            // 返回房間價格
            return rc;
        }


        // POST: BookingDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("bdID,bookingID,roomID,bdAmount,bdTotal")] BookingDetail bookingDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bookingDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["bookingID"] = new SelectList(_context.Bookings, "bookingID", "bookingID", bookingDetail.bookingID);
            ViewData["roomID"] = new SelectList(_context.Rooms, "roomID", "roomID", bookingDetail.roomID);
            return View(bookingDetail);
        }

        // GET: BookingDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookingDetail = await _context.BookingDetails.FindAsync(id);
            if (bookingDetail == null)
            {
                return NotFound();
            }
            ViewData["bookingID"] = new SelectList(_context.Bookings, "bookingID", "bookingID", bookingDetail.bookingID);
            ViewData["roomID"] = new SelectList(_context.Rooms, "roomID", "roomID", bookingDetail.roomID);
            return View(bookingDetail);
        }

        // POST: BookingDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("bdID,bookingID,roomID,bdAmount,bdTotal")] BookingDetail bookingDetail)
        {
            if (id != bookingDetail.bdID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bookingDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingDetailExists(bookingDetail.bdID))
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
            ViewData["bookingID"] = new SelectList(_context.Bookings, "bookingID", "bookingID", bookingDetail.bookingID);
            ViewData["roomID"] = new SelectList(_context.Rooms, "roomID", "roomID", bookingDetail.roomID);
            return View(bookingDetail);
        }

        // GET: BookingDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookingDetail = await _context.BookingDetails
                .Include(b => b.booking)
                .Include(b => b.room)
                .FirstOrDefaultAsync(m => m.bdID == id);
            if (bookingDetail == null)
            {
                return NotFound();
            }

            return View(bookingDetail);
        }

        // POST: BookingDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bookingDetail = await _context.BookingDetails.FindAsync(id);
            if (bookingDetail != null)
            {
                _context.BookingDetails.Remove(bookingDetail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingDetailExists(int id)
        {
            return _context.BookingDetails.Any(e => e.bdID == id);
        }
    }
}
