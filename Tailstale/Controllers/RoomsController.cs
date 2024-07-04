using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tailstale.Hotel_DTO;
using Tailstale.Models;
using Tailstale.Tools;
using WebApplication1.DTO;

namespace Tailstale.Controllers
{
    public class RoomsController : Controller
    {
        private readonly TailstaleContext _context;

        public RoomsController(TailstaleContext context)
        {
            _context = context;
        }

        // GET: Rooms
        public async Task<IActionResult> Index()
        {
            var tailstaleContext = _context.Rooms.Include(r => r.hotel);
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
                .Include(r => r.hotel)
                .FirstOrDefaultAsync(m => m.roomID == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        // GET: Rooms/Create
        public IActionResult Create()
        {
            ViewData["hotelID"] = new SelectList(_context.businesses, "ID", "name");
            return View();
        }

        // POST: Rooms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoomDTO room)
        {
            if (ModelState.IsValid)
            {
                int getID = await GetHotelID();
               
                Room Room = new Room
                {
                    hotelID = getID,
                    roomSpecies=room.roomSpecies,
                    roomType=room.roomType,
                    roomPrice=room.roomPrice,
                    roomDiscount  =room.roomDiscount,
                    roomReserve=room.roomReserve,
                    roomDescrep=room.roomDescrep,
                };
                _context.Rooms.Add(Room);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ShowRoomFromHotel), new { hotelID=getID});
            }
           // ViewData["hotelID"] = new SelectList(_context.businesses, "ID", "name", room.hotelID);
            return View(room);
        }

        // GET: Rooms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }
            
            return View(room);
        }

        // POST: Rooms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Room room)
        {
            int getID = await GetHotelID();
            if (id != room.roomID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    
                    Room editRoom = new Room()
                    {
                        hotelID =getID,
                        roomPrice = room.roomPrice,
                        roomDiscount = room.roomDiscount,
                        roomDescrep = room.roomDescrep,
                        roomID=room.roomID,
                        roomSpecies=room.roomSpecies,
                        roomType=room.roomType,
                        roomReserve=room.roomReserve,
                    };

                    _context.Rooms.Update(editRoom);
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
                return RedirectToAction(nameof(ShowRoomFromHotel), new { hotelID = getID });
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
                .Include(r => r.hotel)
                .FirstOrDefaultAsync(m => m.roomID == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room != null)
            {
                _context.Rooms.Remove(room);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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

                var tailstaleContext = _context.Rooms.Include(r => r.hotel).Where(r=>r.hotelID==hotelID);
                //var businessType = _context.businesses.Where(b => b.ID == hotelID).Select(c => c.name).FirstOrDefault();
               // int hotelID = _context.businesses.Where(b => b.ID == hotelID).Select(c => c.ID).FirstOrDefault();
                string hotelName = _context.businesses.Where(b=>b.ID==hotelID).Select(c=>c.name).FirstOrDefault();
                    HttpContext.Session.SetInt32("hotelID", hotelID);
                    HttpContext.Session.SetString("hotelName", hotelName);
                    return View(tailstaleContext);

            }
            
         
        }
        [HttpPost]

        public async Task<string> GetHotelName()
        {
            //var getSession = _httpContextAccessor.HttpContext.Session.GetString("hotelname");
            string sessionValue = HttpContext.Session.GetString("hotelName");
            //var getSession = "1222";


            return sessionValue;

        }
        public async Task<int> GetHotelID()
        {
            //var getSession = _httpContextAccessor.HttpContext.Session.GetString("hotelname");
            int sessionValue = Convert.ToInt32(HttpContext.Session.GetInt32("hotelID"));
            //var getSession = "1222";


            return sessionValue;

        }

    }
}
