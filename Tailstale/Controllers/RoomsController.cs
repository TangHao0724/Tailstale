using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tailstale.Hotel_DTO;
using Tailstale.Models;

namespace Tailstale.Controllers
{
    public class RoomsController : Controller
    {
        private readonly TailstaleContext _context;
        private readonly IMapper _mapper;

        public RoomsController(TailstaleContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;

        }

        // GET: Rooms
        public async Task<IActionResult> Index()
        {
            var tailstaleContext = _context.Rooms.Include(r => r.FK_roomImg).Include(r => r.FK_roomType).Include(r => r.hotel);
            return View(await tailstaleContext.ToListAsync());
        }

        // GET: Rooms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms
                .Include(r => r.FK_roomImg)
                .Include(r => r.FK_roomType)
                .Include(r => r.hotel)
                .FirstOrDefaultAsync(m => m.roomID == id);
            EditRoomDTO e = ConvertToEditRoomDTO(room);
            if (e == null)
            {
                return NotFound();
            }

            return View(e);
        }

        // GET: Rooms/Create
        public IActionResult Create()
        {
            var hotelID = HttpContext.Session.GetInt32("hotelID11");
           
            ViewData["FK_roomType_ID"] = new SelectList(_context.roomTypes.Where(b => b.FK_businessID == hotelID).Select(h => new {
                TypeId = h.roomType_ID,
                Type = h.roomType1,
            }), "TypeId", "Type");
            return View();
        }

        // POST: Rooms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( RoomDTO room)
        {
            var hotelID = HttpContext.Session.GetInt32("hotelID11");
            if (ModelState.IsValid)
            {
                int InthotelID = Convert.ToInt32(hotelID);
                Room Room = new Room
                {
                    
                    hotelID = InthotelID,
                    roomSpecies = room.roomSpecies,
                    FK_roomType_ID = room.roomType.roomType_ID,
                    roomPrice = room.roomPrice,
                    roomDiscount = room.roomDiscount,
                    roomReserve = room.roomReserve,
                    roomDescrep = room.roomDescrep,
                };

                _context.Rooms.Add(Room);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ShowRoomFromHotel), new { hotelID = InthotelID });
            }
            
            ViewData["FK_roomType_ID"] = new SelectList(_context.roomTypes.Where(b => b.FK_businessID == hotelID).Select(h => new {
                TypeId = h.roomType_ID,
                Type = h.roomType1,
            }), "TypeId", "Type");
            return View(room);
        }

        // GET: Rooms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var hotelID = HttpContext.Session.GetInt32("hotelID11");


            var room = await _context.Rooms.FindAsync(id);
            EditRoomDTO e = ConvertToEditRoomDTO(room);

            if (e == null)
            {
                return NotFound();
            }

            ViewData["FK_roomType_ID"] = new SelectList(_context.roomTypes.Where(b => b.FK_businessID == hotelID).Select(h => new
            {
                TypeId = h.roomType_ID,
                Type = h.roomType1,
            }), "TypeId", "Type", e.roomType.roomType_ID);

            return View(e);
            // return View(map);
        }

        private static EditRoomDTO ConvertToEditRoomDTO(Room? room)
        {
            return new EditRoomDTO
            {
                roomID = room.roomID,
                hotelID = room.hotelID,
                roomPrice = room.roomPrice,
                roomDiscount = room.roomDiscount,
                roomReserve = room.roomReserve,
                roomDescrep = room.roomDescrep,
                roomSpecies = room.roomSpecies,
                roomType = room.FK_roomType,

            };
        }

        // POST: Rooms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditRoomDTO room)
        {
            var hotelID = HttpContext.Session.GetInt32("hotelID11");
            if (id != room.roomID)
            {
                return NotFound();
            }
           
            if (ModelState.IsValid)
            {
                int InthotelID = Convert.ToInt32(hotelID);
                try
                {
                   
                    Room Room = new Room
                    {

                        hotelID = InthotelID,
                        roomSpecies = room.roomSpecies,
                        FK_roomType_ID = room.roomType.roomType_ID,
                        roomPrice = room.roomPrice,
                        roomDiscount = room.roomDiscount,
                        roomReserve = room.roomReserve,
                        roomDescrep = room.roomDescrep,
                    };

                    _context.Rooms.Update(Room);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomExists(room.roomID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ShowRoomFromHotel), new { hotelID = InthotelID });
                
            }
         
           
            return View(room);
        }

        // GET: Rooms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms
                .Include(r => r.FK_roomImg)
                .Include(r => r.FK_roomType)
                .Include(r => r.hotel)
                .FirstOrDefaultAsync(m => m.roomID == id);
            if (room == null)
            {
                return NotFound();
            }
            EditRoomDTO e= ConvertToEditRoomDTO(room);

            return View(e);
        }

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            var hotelID = HttpContext.Session.GetInt32("hotelID11");
            int InthotelID = Convert.ToInt32(hotelID);

            if (room != null)
            {
                _context.Rooms.Remove(room);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ShowRoomFromHotel), new { hotelID = InthotelID });
           
        }

        private bool RoomExists(int id)
        {
            return _context.Rooms.Any(e => e.roomID == id);
        }
        [HttpGet]
        [Route("Rooms/ShowRoomFromHotel/{hotelID:int}")]
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
                return View(tailstaleContext);

            }


        }
        


    }
}
