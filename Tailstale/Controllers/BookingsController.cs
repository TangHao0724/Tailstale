using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Cors;
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
    [EnableCors("Fuen104Policy")]
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
            ViewData["bookingStatus"] = new SelectList(_context.order_statuses.Where(os => os.FK_businessType_ID == 1), "ID", "status_name");
            ViewData["hotelID"] = new SelectList(_context.businesses, "ID", "name");
            ViewData["keeper_ID"] = new SelectList(_context.keepers, "ID", "email");
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Booking booking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["bookingStatus"] = new SelectList(_context.order_statuses.Where(os => os.FK_businessType_ID == 1), "ID", "status_name", booking.bookingStatus);
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
            ViewData["bookingStatus"] = new SelectList(_context.order_statuses.Where(os => os.FK_businessType_ID == 1), "ID", "status_name", booking.bookingStatus);
            ViewData["hotelID"] = new SelectList(_context.businesses, "ID", "name", booking.hotelID);
            ViewData["keeper_ID"] = new SelectList(_context.keepers, "ID", "email", booking.keeper_ID);
            return View(booking);
        }
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
            ViewData["bookingStatus"] = new SelectList(_context.order_statuses.Where(os=>os.FK_businessType_ID==1), "ID", "status_name", booking.bookingStatus);
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

        [HttpPost]
        public async Task<IActionResult> ShowBooking()
        {
            int? a = HttpContext.Session.GetInt32("hotelID11");
            
            if (a != null)
            {
                var booking = _context.Bookings
                    .Include(a => a.bookingStatusNavigation).Include(a => a.hotel).Include(a => a.keeper).Include(a => a.BookingDetails).Where(b => b.hotelID == a).Select(a=>a);
                var map1 = _mappper.Map<IEnumerable<BookingDTO>>(booking);
                return View(map1);
            };

            var bookingDTO = _context.Bookings
                .Include(a => a.bookingStatusNavigation).Include(a => a.hotel).Include(a => a.keeper).Include(a => a.BookingDetails).Select(a => a);

            var map = _mappper.Map<IEnumerable<BookingDTO>>(bookingDTO);

            return View(map);
        }
       
        
        //BookingDetails/ShowBookingDetail/100000
        public async Task<string> ShowBookingDetail(int bookingID)
        {
            var a = _context.BookingDetails.Where(a => a.bookingID == bookingID).Select(a => a);
            
            var map = _mappper.Map<IEnumerable<BookingDetailDTO>>(a);

            return ("123");
        }
        
       
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
                var room = _context.Rooms.Where(o => o.hotelID == h.ID);
                var map = _mappper.Map<IEnumerable<RoomDTO>>(room);
               // EditRoomDTO e = ConvertToEditRoomDTO(room);


                return PartialView("_RoomPartial", map);
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
                    bdAmount = d.bdAmount,
                    //roomName=d.room.,
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
        public async Task<IActionResult> ShowRoomFromHotel(int hotelID)
        {
            if (hotelID == 0)
            {
                return NotFound();
            }
            else
            {
                
                var tailstaleContext = _context.Rooms.Include(r => r.hotel).Include(r => r.FK_roomType).Where(r => r.hotelID == hotelID);
                //var tailstaleContext = _context.Rooms.Include(r => r.hotel).Where(r=>r.hotelID==hotelID);

                string hotelName = _context.businesses.Where(b => b.ID == hotelID).Select(c => c.name).FirstOrDefault();
                HttpContext.Session.SetInt32("hotelID", hotelID);
                HttpContext.Session.SetString("hotelName", hotelName);
                return PartialView("_TestRoom", tailstaleContext);
                

            }


        }
        public async Task<IActionResult> businessList()
        {
            var tailstaleContext = _context.businesses.Include(b => b.business_status).Include(b => b.type);
            return View(await tailstaleContext.ToListAsync());
        }
        [HttpGet]
        [Route("Bookings/ShowRoom/{hotelID:int}")]
        public async Task<IActionResult> ShowRoom(int hotelID)
        {
            if (hotelID == 0)
            {
                return NotFound();
            }
            else
            {
                var tailstaleContext = _context.Rooms.Include(r => r.hotel).Include(r => r.FK_roomType).Where(r => r.hotelID == hotelID);
                //var tailstaleContext = _context.Rooms.Include(r => r.hotel).Where(r=>r.hotelID==hotelID);

                string hotelName = _context.businesses.Where(b => b.ID == hotelID).Select(c => c.name).FirstOrDefault();
                HttpContext.Session.SetInt32("hotelID", hotelID);
                HttpContext.Session.SetString("hotelName", hotelName);
                return View(tailstaleContext);

            }


        }


        //查詢所有剩餘的房間
        //Bookings/SearchRoom
        [HttpGet]
        public async Task<IActionResult> SearchRoom([FromQuery] InputDate iD, int? Cat, int? dog)
        {
            
            var result = RoomAvailabilityAndRoom(iD,Cat,dog);
          // return PartialView("_SearchRoom", finalresult);
            return View(result);
        }
        [HttpGet]
        public async Task<IActionResult>  PostDateToSearch()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PostDateToSearch([FromBody]InputDate iD, int? Cat, int? dog)
        {
            if (iD.startDate <= iD.endDate)
            {
                return RedirectToAction(nameof(SearchRoom),new InputDate
                {
                    startDate= iD.startDate,
                    endDate= iD.endDate,
                });
            }
            return RedirectToAction(nameof(PostDateToSearch));

        }
        //把取得的房間數量和room合併並轉型
        public IEnumerable<FindRoomResultDTO> RoomAvailabilityAndRoom(InputDate iD, int? Cat, int? dog) {
            var result = GetBookedRoomIds(iD.startDate, iD.endDate).ToList();
            var tailstaleContext = _context.Rooms.ToList();
            var finalresult = tailstaleContext.Join(result, t => t.roomID, r => r.RoomId, (tailstaleContext, result) => new FindRoomResultDTO
            {
                roomID = tailstaleContext.roomID,
                roomPrice = (int)tailstaleContext.roomPrice,
                roomDescription = tailstaleContext.roomDescrep,
                roomReserve = result.AvailableCount,
                roomType = tailstaleContext.FK_roomType,
                hotelID = tailstaleContext.hotelID,
                roomSpecies = tailstaleContext.roomSpecies,
                business = tailstaleContext.hotel

            });
            return finalresult;
        }

        //取得剩餘房間數量
        //Bookings/GetBookedRoomIds
        [HttpGet]        
        public IEnumerable<RoomAvailability> GetBookedRoomIds(DateTime startDate, DateTime endDate)
        {
            
            var bookedRoom1 = _context.BookingDetails.Include(b=>b.booking)
                .Where(b=> (b.booking.checkinDate <= endDate && b.booking.checkoutDate >= startDate) ||
                           (b.booking.checkinDate >= startDate && b.booking.checkinDate <= endDate) ||
                           (b.booking.checkoutDate >= startDate && b.booking.checkoutDate <= endDate))
                .GroupBy(b => b.roomID)
                .Select(g => new RoomReseve
                {
                     RoomId = g.Key,
                     Quantity = Convert.ToInt32(g.Sum(x => x.bdAmount))
                 });
            // 查詢所有可用的房間
            var allRooms = _context.Rooms
                .Select(r => new RoomReseve
                {
                    RoomId = r.roomID,
                    Quantity = Convert.ToInt32(r.roomReserve)
                });

            // 計算剩餘房間數量
            var bookedRoomsList = bookedRoom1.ToList();
            var allRoomsList = allRooms.ToList();

            var availableRooms = allRoomsList.GroupJoin(
                bookedRoomsList,
                r => r.RoomId,
                b => b.RoomId,
                (r, b) => new RoomAvailability
                {
                    RoomId = r.RoomId,
                    AvailableCount = r.Quantity - (b.FirstOrDefault() != null ? b.FirstOrDefault().Quantity : 0)
                });
           
            return availableRooms;
            //return bookedRoom1;

        }




    }
}
