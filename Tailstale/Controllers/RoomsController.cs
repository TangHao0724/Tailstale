using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tailstale.Hotel_DTO;
using Tailstale.Models;

namespace Tailstale.Controllers
{
    [EnableCors("Fuen104Policy")]
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
        public IActionResult AddOrEdit(int id=0)
        {
            var hotelID = HttpContext.Session.GetInt32("hotelID11");
            //var c = _context.Rooms.Include(r => r.FK_roomType);
            ViewData["FK_roomType_ID"] = new SelectList(_context.roomTypes.Where(b => b.FK_businessID == hotelID).Select(h => new {
                TypeId = h.roomType_ID,
                Type = h.roomType1,
            }), "TypeId", "Type");
            ViewBag.HotelID=hotelID;
            if (id == 0)
                return View(new RoomDTO());
            else
            {
                var getroom = _context.Rooms.Find(id);
                RoomDTO roomDTO = new RoomDTO
                {
                    hotelID = getroom.hotelID,
                    
                    roomID=getroom.roomID,
                    roomSpecies=getroom.roomSpecies,
                    roomPrice= getroom.roomPrice,
                    roomDiscount= getroom.roomDiscount,
                    roomReserve= getroom.roomReserve,
                    roomDescrep= getroom.roomDescrep,
                    roomType= getroom.FK_roomType,
                };

                return View(roomDTO);
            }
                
        }

        // POST: Rooms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit( RoomDTO room)
        {
            var hotelID = HttpContext.Session.GetInt32("hotelID11");
            if (ModelState.IsValid)
            {
                
                int InthotelID = Convert.ToInt32(hotelID);
                Room Room = new Room
                {
                    roomID=(int)room.roomID,
                    hotelID = InthotelID,
                    roomSpecies = room.roomSpecies,
                    FK_roomType_ID = room.roomType.roomType_ID,
                    roomPrice = room.roomPrice,
                    roomDiscount = room.roomDiscount,
                    roomReserve = room.roomReserve,
                    roomDescrep = room.roomDescrep,
                };

                if (room.roomID == 0)
                {
                    
                    _context.Rooms.Add(Room);
                }
                else
                {
                    _context.Rooms.Update(Room);
                }
                    
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ShowRoomFromHotel), new { hotelID = InthotelID });
            }
            
            ViewData["FK_roomType_ID"] = new SelectList(_context.roomTypes.Where(b => b.FK_businessID == hotelID).Select(h => new {
                TypeId = h.roomType_ID,
                Type = h.roomType1,
            }), "TypeId", "Type");
            return View(room);
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

        
        
        [HttpPost, ActionName("ShowRoomFromHotel")]
        //[Route("Rooms/ShowRoomFromHotel/{hotelID:int}")]
        public async Task<IActionResult> ShowRoomFromHotel(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            else
            {
                var tailstaleContext = _context.Rooms.Include(r => r.hotel).Include(r => r.FK_roomType).Where(r => r.hotelID == id);
                //var tailstaleContext = _context.Rooms.Include(r => r.hotel).Where(r=>r.hotelID==hotelID);

                string hotelName = _context.businesses.Where(b => b.ID == id).Select(c => c.name).FirstOrDefault();
                HttpContext.Session.SetInt32("hotelID", id);
                HttpContext.Session.SetString("hotelName", hotelName);
                return View(tailstaleContext);

            }


        }

        //Rooms/GetPosition

        public async Task<XElement> GetPosition()
        {
            var address = "高雄市岡山區大莊路80巷";
            var url = String.Format("http://maps.google.com/maps/api/geocode/json?sensor=false&address={0}", Uri.EscapeDataString(address));

            var request = WebRequest.Create(url);
            var response = request.GetResponse();
            var xdoc = XDocument.Load(response.GetResponseStream());

            var result = xdoc.Element("GeocodeResponse").Element("result");
            var locationElement = result.Element("geometry").Element("location");
            var lat = locationElement.Element("lat");
            var lng = locationElement.Element("lng");

            return lng;
        }

    }
}
