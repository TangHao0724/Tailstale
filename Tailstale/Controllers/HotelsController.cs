using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Tailstale.Hotel_DTO;
using Tailstale.Models;
using Tailstale.Tools;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Tailstale.Controllers
{
    [EnableCors("Fuen104Policy")]
    public class HotelsController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly TailstaleContext _context;
        private readonly IMapper _mapper;

       
        public HotelsController(TailstaleContext context, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;

        }

        public IActionResult Index()
        {
            return View();
        }

        //業主入口
        public async Task<IActionResult> BusinessIndex()
        {

            var tailstaleContext = _context.businesses.Include(b => b.business_statusNavigation).Include(b => b.type);
            return View(await tailstaleContext.ToListAsync());
        }

        [HttpGet]
        [Route("Hotels/businesslogin/{hotelID:int}")]
        public async Task<IActionResult> businesslogin(int hotelID)
        {
            business b = _context.businesses.Where(b => b.ID == hotelID).FirstOrDefault();
            var hotelName = _context.businesses.Where(b => b.ID == hotelID).Select(b => b.name).FirstOrDefault();
            ViewBag.hotelID = hotelID;
            HttpContext.Session.SetInt32("hotelID11", hotelID);
            HttpContext.Session.SetString("hotelName11", hotelName);
            return View();
        }


        // GET: Rooms/Details/5
        public async Task<IActionResult> RoomDetails(int? id)
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
            EditRoomDTO e = ConvertToEditRoomDTO(room, img);
            if (e == null)
            {
                return NotFound();
            }

            return View(e);
        }

        // GET: Hotels/RoomAddOrEdit
        public IActionResult RoomAddOrEdit(int id = 0)
        {
            var hotelID = HttpContext.Session.GetInt32("hotelID11");
            //var c = _context.Rooms.Include(r => r.FK_roomType);
            ViewData["FK_roomType_ID"] = new SelectList(_context.roomTypes.Where(b => b.FK_businessID == hotelID).Select(h => new {
                TypeId = h.roomType_ID,
                Type = h.roomType1,
            }), "TypeId", "Type");
            ViewBag.HotelID = hotelID;
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

        //新增或編輯ROOM
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RoomAddOrEdit(RoomDTO room, IList<IFormFile>? files)
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

                    if (files != null && files.Count > 0)
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

        //RoomDto轉Room
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

        //新增照片類型
        private static business_img_type AddBusinessImgType(int InthotelID)
        {
            return new business_img_type
            {
                FK_business_id = InthotelID,
                typename = "123",
                created_at = DateTime.Now
            };
        }

        //將照片上傳到wwwroot
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


        //Room轉roomDTO
        private static EditRoomDTO ConvertToEditRoomDTO(Room room, List<business_img> img)
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
                roomImg = img


            };
        }

        //刪除room
        [HttpPost, ActionName("RoomDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RoomDeleteConfirmed(int id)
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


        //顯示業者的房間
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


        //booking相關

        //查詢所有剩餘的房間
        //Hotels/SearchRoom
        [HttpGet]
        public async Task<IActionResult> SearchHotels([FromQuery] InputDate iD, int? Cat, int? Dog, string? address)
        {
            var cookie = new
            {
                startdate = iD.startDate.ToString("yyyy-MM-dd"),
                enddate = iD.endDate.ToString("yyyy-MM-dd"),
                cat = Cat == null ? 0 : Cat,
                dog = Dog == null ? 0 : Dog
            };
            ViewBag.Cookie = cookie;
            var result = await RoomAvailabilityAndRoom(iD, Cat, Dog, address);
            var dateCount = ViewBag.totalDays;

            var hotels =result.GroupBy(h => h.hotelID).Select(h => h.Key).ToList();
            // var hotelslist = _context.businesses.Where(h =>( result.GroupBy(h => h.hotelID).Select(h => h.Key)).Contains(h.ID)).ToList();

            var hotelslist = _context.businesses.AsNoTracking().Where(h => hotels.Contains(h.ID)).ToList();

            //var findhotels = _context.businesses.Join(result, b => b.ID, r => r.hotelID,).ToList();
            //var findhotels = _context.businesses.Join(result, b => b.ID, r => r.hotelID, (b, r) => new hotelResult
            //{
            //    businesse = b,
            //    roomPrice = r.roomPrice

            //}).Where(h => hotels.Contains(h.businesse.ID)).ToList();
            //var findhotels = _context.businesses.Join(result, b => b.ID, r => r.hotelID, (b, r) => new hotelResult
            //{
            //    businesse = b,
            //    roomPrice = r.roomPrice

            //}).Where(h => hotels.Contains(h.businesse.ID)).ToList();

            //var findhotels = _context.businesses
            //                .GroupJoin(result,
            //                    b => b.ID,
            //                    r => r.hotelID,
            //                    (b, roomGroup) => new
            //                    {
            //                        businesse = b,
            //                        Rooms = roomGroup // 這裡是分組的房間
            //                    })
            //                .SelectMany(
            //                    x => x.Rooms.DefaultIfEmpty(), // 如果沒有房間，則返回一個空的房間
            //                    (x, r) => new hotelResult
            //                    {
            //                        businesse = x.businesse,
            //                        roomPrice = (int)r.priceTotal // 使用條件運算符以避免空值
            //                    })
            //                .Where(h => hotels.Contains(h.businesse.ID)) // 確保使用正確的屬性比對
            //                .ToList();

            if(Cat!=null || Dog != null)
            {
                var resultgroupbyhotel = result.GroupBy(r => r.hotelID).Select(r => new
                {
                    hotelID = r.Key,
                    price = r.Select(r => r.priceTotal).FirstOrDefault(),
                    date = dateCount,
                    onedatePrice = r.Select(r => r.priceTotal).FirstOrDefault() / dateCount ,
                });
                var finalresult = hotelslist.Join(resultgroupbyhotel, b => b.ID, r => r.hotelID, (b, r) => new hotelResult
                {
                    businesse = b,
                    roomPrice = r.price,
                    date = r.date,
                    onedatePrice = r.onedatePrice 

                }).ToList();
                return View(finalresult);

            }
            var noCatDog = hotelslist.Select(h=> new hotelResult
            {
                businesse = h,
                date = dateCount,
               
            });


            //var hotelslist=_context.businesses.Where(h=>hotels.Contains(h.ID)).ToList();


            

            // return PartialView("_SearchRoom", finalresult);
            return View(noCatDog);
        }

        [HttpGet]
        public async Task<IActionResult> SearchRoom(int hotelID)
        {
            var getMyResult = ViewBag.getRoomList;

            //var result = await RoomAvailabilityAndRoom(iD, Cat, Dog, address);
            // var hotels = await result.GroupBy(h => h.hotelID).Select(h => h.Key).ToList();
            // var findhotels = _context.businesses.Where(h => hotelID==h.ID).ToList();


            // return PartialView("_SearchRoom", finalresult);
            //return View(findhotels);
            return null;
        }
        //[HttpGet]
        //public async Task<IActionResult> SearchRoom([FromQuery] InputDate iD, int? Cat, int? Dog, string? address, int hotelID)
        //{
        //    var getMyResult = ViewBag.getRoomList;

        //    //var result = await RoomAvailabilityAndRoom(iD, Cat, Dog, address);
        //    // var hotels = await result.GroupBy(h => h.hotelID).Select(h => h.Key).ToList();
        //    // var findhotels = _context.businesses.Where(h => hotelID==h.ID).ToList();


        //    // return PartialView("_SearchRoom", finalresult);
        //    //return View(findhotels);
        //    return null;
        //}

        [HttpGet]
        public async Task<IActionResult> PostDateToSearch()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PostDateToSearch([FromBody] InputDate iD, int? Cat, int? dog, string? address)
        {
            if (iD.startDate <= iD.endDate)
            {

                return RedirectToAction(nameof(SearchRoom), new InputDate
                {
                    startDate = iD.startDate,
                    endDate = iD.endDate,


                });
            }
            return RedirectToAction(nameof(PostDateToSearch));

        }

        //把取得的房間數量和room合併並轉型
        public async Task<IEnumerable<FindRoomResultDTO>> RoomAvailabilityAndRoom(InputDate iD, int? Cat, int? dog, string? address)
        {
            var result = await GetBookedRoomIds(iD.startDate, iD.endDate);

            

            // 計算日期差
            TimeSpan dateCount = iD.endDate - iD.startDate;

            // 取得天數並轉換為整數
            int totalDays = (int)dateCount.TotalDays;
            ViewBag.totalDays = totalDays;

            //  var dateCount = (iD.endDate - iD.startDate);
            var tailstaleContext = address == null || address == ""
                                   ? await _context.Rooms.AsNoTracking().ToListAsync()
                                   : await _context.Rooms.AsNoTracking().Include(r => r.hotel)
                                   .Where(r => r.hotel.address.Contains(address)).ToListAsync();

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

            }).ToList();

            if (Cat < 0 && dog < 0)
            {
                ViewBag.getRoomList = finalresult;
                return finalresult;
            }
            else
            {
                List<RoomSearchResult> getCatroom = Cat == null ? null : getSpeciesRoom(Cat, finalresult, "貓");
                List<RoomSearchResult> getDogroom = dog == null ? null : getSpeciesRoom(dog, finalresult, "狗");

                if (getCatroom != null || getDogroom != null)
                {
                    var catroomPrice = getCatroom == null ? Enumerable.Empty<HotelPrice>() : getMinRoomPrice1(getCatroom, (int)Cat);
                    var dogroomPrice = getDogroom == null ? Enumerable.Empty<HotelPrice>() : getMinRoomPrice1(getDogroom, (int)dog);
                    var roomPriceTotal = catroomPrice.Concat(dogroomPrice ?? Enumerable.Empty<HotelPrice>())
                                      .GroupBy(hp => hp.HotelID)
                                      .Select(group => new HotelPrice
                                      {
                                          HotelID = group.Key,
                                          Price = group.Sum(hp => hp.Price)
                                      })
                                      .ToList();


                    var roomPriceDict = roomPriceTotal.ToDictionary(p => p.HotelID, p => p.Price);

                    finalresult = finalresult.Select(f => new FindRoomResultDTO
                    {
                        roomID = f.roomID,
                        roomPrice = f.roomPrice,
                        roomDescription = f.roomDescription,
                        roomReserve = f.roomReserve,
                        roomType = f.roomType,
                        hotelID = f.hotelID,
                        roomSpecies = f.roomSpecies,
                        business = f.business,
                        priceTotal = roomPriceDict.TryGetValue(f.hotelID, out var price) ? price * totalDays : 0 // 使用字典查找
                    }).ToList();
                }


                var catRoomIds = getCatroom == null ? null : getCatroom.Select(c => c.roomID).ToHashSet();
                var dogRoomIds = getDogroom == null ? null : getDogroom.Select(c => c.roomID).ToHashSet();

                if (Cat > 0 && dog > 0)
                {
                    finalresult = finalresult.Where(r => catRoomIds.Contains(r.roomID) || dogRoomIds.Contains(r.roomID)).ToList();
                }
                else if (Cat > 0)
                {
                    finalresult = finalresult.Where(r => catRoomIds.Contains(r.roomID)).ToList();
                }
                else if (dog > 0)
                {
                    finalresult = finalresult.Where(r => dogRoomIds.Contains(r.roomID)).ToList();
                }
                ViewBag.getRoomList = finalresult;
                return finalresult;
            }


            
        }

        private static List<HotelPrice> getMinRoomPrice1(List<RoomSearchResult> getRoom, int theRoomCount)
        {
            var hotelPrices = new List<HotelPrice>();

            // 按 hotelID 分組
            var groupedRooms = getRoom.GroupBy(g => g.hotelID);

            foreach (var hotelGroup in groupedRooms)
            {
                int hotelTotalPrice = 0;
                int remainingRoomCount = theRoomCount;

                // 對每個酒店的房間按價格升序排序
                var sortedRooms = hotelGroup.OrderBy(r => r.roomPrice).ToList();

                foreach (var room in sortedRooms)
                {
                    if (remainingRoomCount <= 0)
                        break;

                    if (room.roomReserve > 0)
                    {
                        int getMinNum = Math.Min(room.roomReserve, remainingRoomCount);
                        hotelTotalPrice += getMinNum * room.roomPrice;
                        remainingRoomCount -= getMinNum;
                    }
                }

                // 只有當酒店能夠提供至少一個房間時，才添加到結果中
                if (hotelTotalPrice > 0)
                {
                    hotelPrices.Add(new HotelPrice
                    {
                        HotelID = hotelGroup.Key,
                        Price = hotelTotalPrice
                    });
                }
            }

            return hotelPrices;
        }

        
        private static int getMinRoomPrice(List<RoomSearchResult> getRoom, int theRoomCount)
        {
            int totalPrice = 0;
            foreach (var room in getRoom)
            {
                if (theRoomCount <= 0)
                    break;
                getRoom.GroupBy(g => g.hotelID).Select(g=>g);
                if (room.roomReserve > 0)
                {
                    int getMinNum = Math.Min(room.roomReserve, theRoomCount);
                    totalPrice += getMinNum * room.roomPrice;
                    theRoomCount -= getMinNum;
                    room.roomReserve -= getMinNum;
                }
            }
            return totalPrice;
            
        }

        private static List<RoomSearchResult> getSpeciesRoom(int? dog, IEnumerable<FindRoomResultDTO> finalresult, string Species)
        {
            return finalresult.GroupBy(r => new
            {
                r.hotelID,
                r.roomSpecies
            }).Where(r =>
            r.Key.roomSpecies == Species && r.Sum(g => g.roomReserve) >= dog)
           .SelectMany(g => g.Select(g => new RoomSearchResult
           {
               roomID = g.roomID,
               roomPrice = g.roomPrice,
               roomReserve = g.roomReserve,
               hotelID=g.hotelID
           })).OrderBy(r=>r.roomPrice)
           .ToList();
        }

       

        //取得剩餘房間數量 無分物種
        //Bookings/GetBookedRoomIds
        [HttpGet]
        public async  Task<IEnumerable<RoomAvailability>> GetBookedRoomIds(DateTime startDate, DateTime endDate)
        {

            var bookedRoom1 = _context.BookingDetails.AsNoTracking().Include(b => b.booking)
                .Where(b => (b.booking.checkinDate <= endDate && b.booking.checkoutDate >= startDate) ||
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
            var bookedRoomsList =await bookedRoom1.ToListAsync();
            var allRoomsList = await allRooms.ToListAsync();

            var availableRooms = allRoomsList.GroupJoin(
                bookedRoomsList,
                r => r.RoomId,
                b => b.RoomId,
                (r, b) => new RoomAvailability
                {
                    RoomId = r.RoomId,
                    AvailableCount = r.Quantity - (b.FirstOrDefault() != null ? b.FirstOrDefault().Quantity : 0)
                });

            return  availableRooms;
            //return bookedRoom1;

        }
        [HttpPost]
        public async Task<IActionResult> ShowBooking()
        {
            int? a = HttpContext.Session.GetInt32("hotelID11");

            if (a != null)
            {
                var booking = _context.Bookings
                    .Include(a => a.bookingStatusNavigation).Include(a => a.hotel).Include(a => a.keeper).Include(a => a.BookingDetails).Where(b => b.hotelID == a).Select(a => a);
                var map1 = _mapper.Map<IEnumerable<BookingDTO>>(booking);
                return View(map1);
            };

            var bookingDTO = _context.Bookings
                .Include(a => a.bookingStatusNavigation).Include(a => a.hotel).Include(a => a.keeper).Include(a => a.BookingDetails).Select(a => a);

            var map = _mapper.Map<IEnumerable<BookingDTO>>(bookingDTO);

            return View(map);
        }

       
    }
}
