using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tailstale.Hotel_DTO;
using Tailstale.Hotel_ViewModels;
using Tailstale.Models;
using Tailstale.Tools;
using WebApplication1.DTO;

namespace Tailstale.Controllers
{
   
    public class BookingsController : Controller
    {
        private readonly TailstaleContext _context;
        private readonly IMapper _mappper;
        private readonly IHttpContextAccessor _httpContextAccessor;
       
        public BookingsController(TailstaleContext context,IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mappper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

       // GET: Bookings
        public async Task<IActionResult> Index()
        {
            var tailstaleContext = _context.Bookings.Include(a => a.bookingStatusNavigation).Include(a => a.hotel).Include(a => a.keeper).AsNoTracking();
            return View(tailstaleContext);

        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.bookingStatusNavigation)
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
            ViewData["bookingStatus"] = new SelectList(_context.order_statuses, "ID", "status_name");
            ViewData["hotelID"] = new SelectList(_context.businesses, "ID", "name");
            ViewData["keeper_ID"] = new SelectList(_context.keepers, "ID", "email");
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("bookingID,keeper_ID,hotelID,checkinDate,checkoutDate,bookingAmountTotal,bookingStatus,bookingDate")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["bookingStatus"] = new SelectList(_context.order_statuses, "ID", "status_name", booking.bookingStatus);
            ViewData["hotelID"] = new SelectList(_context.businesses, "ID", "name", booking.hotelID);
            ViewData["keeper_ID"] = new SelectList(_context.keepers, "ID", "email", booking.keeper_ID);
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
            ViewData["bookingStatus"] = new SelectList(_context.order_statuses, "ID", "status_name", booking.bookingStatus);
            ViewData["hotelID"] = new SelectList(_context.businesses, "ID", "name", booking.hotelID);
            ViewData["keeper_ID"] = new SelectList(_context.keepers, "ID", "email", booking.keeper_ID);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Booking booking)
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
            ViewData["bookingStatus"] = new SelectList(_context.order_statuses, "ID", "status_name", booking.bookingStatus);
            ViewData["hotelID"] = new SelectList(_context.businesses, "ID", "name", booking.hotelID);
            ViewData["keeper_ID"] = new SelectList(_context.keepers, "ID", "email", booking.keeper_ID);
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
                .Include(b => b.bookingStatusNavigation)
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
        

        [HttpGet,ActionName("ShowBooking")]
        public async Task<IActionResult> ShowBooking()
        {
            var bookingDTO = _context.Bookings
                .Include(a => a.bookingStatusNavigation).Include(a => a.hotel).Include(a => a.keeper).Include(a => a.BookingDetails).Select(a => a);
           var map =_mappper.Map<IEnumerable<BookingDTO>>(bookingDTO);

            return View(map);
        }
        //BookingDetails/ShowBookingDetail/100000
       
        public async Task<string> ShowBookingDetail(int bookingID)
        {
            var a = _context.BookingDetails.Where(a => a.bookingID == bookingID).Select(a => a);
            
            var map = _mappper.Map<IEnumerable<BookingDetailDTO>>(a);

            return ("123");
        }
        
        public class BookingViewModelConvert
        {
            public static BookingViewModel BVM(BookingDTO dto)
            {
                return new BookingViewModel
                {
                    BookingID = dto.BookingID,
                    HotelName = dto.HotelName,
                    KeeperName = dto.KeeperName,
                    BookingStatus = dto.BookingStatus,
                    CheckinDate = dto.CheckinDate,
                    CheckoutDate = dto.CheckoutDate,
                    BookingDate = dto.BookingDate,
                    BookingTotal = dto.BookingTotal,
                    BDVM = ConvertBDDTOToVM(dto.BookingDetailDTOs)

                };

            }
            private static List<BookingDetailViewModel> ConvertBDDTOToVM(List<BookingDetailDTO> BDDTO)
            {
                return BDDTO.Select(bd => new BookingDetailViewModel
                {
                    bookingID = bd.bookingID,
                    roomName = bd.roomName,
                    bdAmount = bd.bdAmount,
                    bdTotal = bd.bdTotal,
                }).ToList();
            }
        }

        //Bookings/CreateBooking
        public async Task<IActionResult> CreateBooking()
        {
            ViewBag.Hotel = new SelectList(_context.businesses.Where(b=>b.type_ID==1).Select(h => new {
            HotelId = h.ID,
            HotelName = h.name,           
            }), "HotelId", "HotelName");

            return View();
        }
        [HttpGet]
        [Route("Bookings/Room/{hotelID:int}")]
        public async Task<IActionResult> Room(int hotelID)
        {
           
            business h = await _context.businesses.FindAsync(hotelID);
            if (h == null)
            {
                return NotFound();
            }
            else
            {
                //string hotelName = h.name;
                //HttpContext.Session.SetString("hotelName", hotelName);
                
               
                return PartialView("_RoomPartial", _context.Rooms.Where(o => o.hotelID == h.ID) );
            }
        }
       


        private static BookingDTO MyBookingDTO(Booking a) 
        {
            List<BookingDetailDTO> detail = new List<BookingDetailDTO>();
            foreach(var d in a.BookingDetails)
            {
                BookingDetailDTO bd = new BookingDetailDTO 
                {
                    bookingID = d.bookingID,
                    roomName = d.room.roomType,
                    bdAmount = d.bdAmount,
                    bdTotal = d.bdTotal,
                };
                detail.Add(bd);
            }
            return new BookingDTO
            {
                BookingID = a.bookingID,
                KeeperName = a.keeper.name,
                HotelName = a.hotel.name,
                CheckinDate = a.checkinDate.Value,
                CheckoutDate = a.checkoutDate.Value,
                BookingStatus = a.bookingStatusNavigation.status_name,
                BookingTotal = a.bookingAmountTotal.Value,
                BookingDate = a.bookingDate.Value,
                BookingDetailDTOs = detail,
            };



        }
        public  int? CalPrice(BookingDetailDTO bdDTO)
        {
            if(bdDTO ==null)
            {
                return 0;
            }
            
            int? total = bdDTO.roomPrice * bdDTO.bdAmount;

            return total;
        } 
        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.bookingID == id);
        }
    }
}
