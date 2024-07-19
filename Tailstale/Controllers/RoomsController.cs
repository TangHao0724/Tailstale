﻿using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
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
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly TailstaleContext _context;
        private readonly IMapper _mapper;

        public RoomsController(TailstaleContext context,IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;

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
            var img = _context.business_img_types.Include(bti => bti.Rooms).Include(bti => bti.business_imgs).Where(bti => bti.ID == room.FK_roomImg_ID).SelectMany(bti => bti.business_imgs).ToList();
            EditRoomDTO e = ConvertToEditRoomDTO(room,img);
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
                //var map = _mapper.Map<IEnumerable<RoomDTO>>(getroom);
                RoomDTO roomDTO = new RoomDTO
                {
                    hotelID = getroom.hotelID,
                    roomID = getroom.roomID,
                    roomSpecies = getroom.roomSpecies,
                    roomPrice = getroom.roomPrice,
                    roomDiscount = getroom.roomDiscount,
                    roomReserve = getroom.roomReserve,
                    roomDescrep = getroom.roomDescrep,
                    roomType = getroom.FK_roomType,
                   
                };
                if (getroom.FK_roomImg != null)
                {
                    roomDTO.roomImg = getroom.FK_roomImg.business_imgs.Where(roomimage => roomimage.img_type_id == getroom.FK_roomImg_ID).Select(r => r).ToList();
                }
                else
                {
                    roomDTO.roomImg = null;
                }

                return View(roomDTO);
            }
                
        }

        // POST: Rooms/AddOrEdit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit( RoomDTO room, IList<IFormFile>? files)
        {
            var hotelID = HttpContext.Session.GetInt32("hotelID11");
            if (ModelState.IsValid)
            {
               
                int InthotelID = Convert.ToInt32(hotelID);
              
                //新增新Room
                if (room.roomID == null)
                {
                    //新增imgtype 並處存取得typeID
                    business_img_type imgtype = AddBusinessImgType(InthotelID);
                    _context.business_img_types.Add(imgtype);
                    _context.SaveChanges();

                    Room RoomCreate = RoomConvertRoomDTO(room, InthotelID);
                    RoomCreate.FK_roomImg_ID = imgtype.ID;
                    //FK_roomImg_ID = imgtype.ID
                    _context.Rooms.Add(RoomCreate);
                    await _context.SaveChangesAsync();

                    if (files.Count > 0)
                    {

                        foreach (var file in files)
                        {

                            newimgColAndAddImg(imgtype, RoomCreate, file);
                        }
                    }

                }
                else
                {
                    var isRoomImgType = _context.Rooms.Where(r => r.roomID == room.roomID).Select(r => r.FK_roomImg_ID).FirstOrDefault();
                    Room RoomEdit = RoomConvertRoomDTO(room, InthotelID);
                    RoomEdit.roomID = (int)room.roomID;

                    //_context.Rooms.Update(RoomEdit);
                    //await _context.SaveChangesAsync();

                    if (files != null && files.Count>0)
                    {
                        if (isRoomImgType == null) {
                            business_img_type imgtype = AddBusinessImgType(InthotelID);
                            _context.business_img_types.Add(imgtype);
                            _context.SaveChanges();

                            RoomEdit.FK_roomImg_ID = imgtype.ID;

                            _context.Update(RoomEdit);
                            //await _context.SaveChangesAsync();

                            foreach (var file in files)
                            {
                                newimgColAndAddImg(imgtype, RoomEdit, file);
                            }
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            var imgtype = _context.business_img_types.Where(b => b.ID == isRoomImgType).FirstOrDefault();



                            foreach (var file in files)
                            {
                                newimgColAndAddImg(imgtype, RoomEdit, file);
                            }
                            await _context.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        _context.Rooms.Update(RoomEdit);
                        await _context.SaveChangesAsync();
                    }
                        
                    
                    
                    
                }
                    
              //  await _context.SaveChangesAsync();
             
                return RedirectToActionPreserveMethod(nameof(ShowRoomFromHotel), "Rooms", new { id = InthotelID });
            }

            ViewData["FK_roomType_ID"] = new SelectList(_context.roomTypes.Where(b => b.FK_businessID == hotelID).Select(h => new
            {
                TypeId = h.roomType_ID,
                Type = h.roomType1,
            }), "TypeId", "Type");
            return View(room);
        }

        private static Room RoomConvertRoomDTO(RoomDTO room, int InthotelID)
        {
            return new Room
            {
                hotelID = InthotelID,
                roomSpecies = room.roomSpecies,
                FK_roomType_ID = room.roomType.roomType_ID,
                roomPrice = room.roomPrice,
                roomDiscount = room.roomDiscount,
                roomReserve = room.roomReserve,
                roomDescrep = room.roomDescrep,

            };
        }

        private static business_img_type AddBusinessImgType(int InthotelID)
        {
            return new business_img_type
            {
                FK_business_id = InthotelID,
                typename = "123",
                created_at = DateTime.Now
            };
        }

        private void newimgColAndAddImg(business_img_type imgtype, Room RoomCreate, IFormFile file)
        {
            var imgcol = new business_img
            {
                img_type_id = imgtype.ID,
                name = (RoomCreate.roomID).ToString(),
                created_at = DateTime.Now
            };

            string wwwRootPath = _webHostEnvironment.WebRootPath;

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string filefoldername = (string)imgcol.name;
            string roomImgPath = $@"images\room\{filefoldername}";
            string getPath = Path.Combine(wwwRootPath, roomImgPath);

            if (!Directory.Exists(getPath))
            {
                Directory.CreateDirectory(getPath);
            }

            using (var fileStream = new FileStream(Path.Combine(getPath, fileName), FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            imgcol.URL = roomImgPath + @"\" + fileName;

            _context.business_imgs.Add(imgcol);
        }

        //rooms/test
      
        private static EditRoomDTO ConvertToEditRoomDTO(Room room,List<business_img> img)
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
                roomImg=img


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
                if (room.FK_roomImg_ID != null)
                {
                    var ImageTypeIDTodelete = (int)room.FK_roomImg_ID;
                    var ImageTypeToDelete = _context.business_img_types.Find(ImageTypeIDTodelete);
                    var ImageToDelete = _context.business_imgs.Where(img => img.img_type_id == ImageTypeIDTodelete).ToList();

                    string imgFolder = ImageTypeIDTodelete.ToString();
                    string imageFolder = @"images\room\" + imgFolder;

                    var toDeleteImageFolder = Path.Combine(_webHostEnvironment.WebRootPath, imageFolder.Trim());
                    if (Directory.Exists(toDeleteImageFolder))
                    {
                        Directory.Delete(toDeleteImageFolder, true);
                    }
                    
                    foreach (var img in ImageToDelete)
                    {
                        _context.business_imgs.Remove(img);
                    }

                    _context.business_img_types.Remove(ImageTypeToDelete);
                };
                    _context.Rooms.Remove(room);
            }

            await _context.SaveChangesAsync();
            return RedirectToActionPreserveMethod(nameof(ShowRoomFromHotel), "Rooms", new { id = InthotelID });
          
           
        }

        
        
        [HttpPost, ActionName("ShowRoomFromHotel")]
       // [Route("Rooms/ShowRoomFromHotel?hotelID={hotelID:int}")]
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
