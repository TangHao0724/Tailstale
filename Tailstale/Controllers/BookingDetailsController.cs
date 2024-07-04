using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tailstale.Hotel_DTO;
using Tailstale.Models;
using Tailstale.Tools;

namespace Tailstale.Controllers
{
   // [Route("api/[controller]")]
    [EnableCors("Fuen104Policy")]
    public class BookingDetailsController : Controller
    {
        private readonly TailstaleContext _context;
        private readonly IMapper _mapper;

        public BookingDetailsController(TailstaleContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: BookingDetails
        public async Task<IActionResult> Index()
        {
            var tailstaleContext = _context.BookingDetails.Include(b => b.booking).Include(b => b.room).AsNoTracking();
            return View(tailstaleContext);
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
        public async Task<int?> GetRoomPrice([FromBody] BDInsert ID)
        {
            // 根據 roomID 從資料庫或其他數據源中查詢房間價格


            int? rc = _context.Rooms.Where(c => c.roomID == ID.roomID).Select(r => r.roomPrice).FirstOrDefault();
            int? roomPriceTotal = rc * ID.roomAmount;

            // Console.WriteLine(rc);  
            // 返回房間價格
            return roomPriceTotal;
        }
        //BookingDetails/ShowBookingD/100000
        [Route("BookingDetails/ShowBookingD/{bookingID:int}")]
        public async Task<IActionResult> ShowBookingD(int bookingID)
        {
            
            var bookingDTO = _context.BookingDetails.Include(b => b.room).Where(b=>b.bookingID== bookingID).Select(b => b).AsNoTracking();
            var map = _mapper.Map<IEnumerable<BookingDetailDTO>>(bookingDTO);

            return View(map);
        }
        //public async Task<IActionResult> ShowBookingDOne(int bookingID)
        //{
        //    if (bookingID == null)
        //    {
        //        return NotFound("找不到資料");
        //    }
        //    var bookingDTO = _context.BookingDetails.Include(b => b.room).Select(b => b).AsNoTracking();
        //    if(bookingDTO == null)
        //    {
        //        return NotFound("沒有資料");
        //    }
        //    var map = _mapper.Map<IEnumerable<BookingDetailDTO>>(bookingDTO);
           

        //    return View(map);
        //}


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
