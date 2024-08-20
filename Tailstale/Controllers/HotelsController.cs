using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Text.Json;
using NuGet.Packaging;
using Tailstale.Hotel_DTO;
using Tailstale.Models;
using Tailstale.Tools;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Moq;
using Microsoft.Build.Experimental.FileAccess;
using Microsoft.Extensions.Options;
using Microsoft.CodeAnalysis.CSharp;
using System.Security.Cryptography.Xml;
using ECPay.Payment.Integration;
using System.ComponentModel;



namespace Tailstale.Controllers
{
    [EnableCors("Fuen104Policy")]

    public class HotelsController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly TailstaleContext _context;
       // private readonly ECPaySettings _ecPaySettings;
        private readonly IMapper _mapper;
        public static List<FindRoomResultDTO> getMyResult;
        //public static int businessID;
        //public static int businessType;

        public HotelsController(TailstaleContext context, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
           
        }
        //public HotelsController(TailstaleContext context, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        //{
        //    _context = context;
        //    _mapper = mapper;
        //    _webHostEnvironment = webHostEnvironment;

        //}

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
        public async Task<IActionResult> businesslogin()
        {
            // var a = ViewBag.loginID;
            var businessID = HttpContext.Session.GetInt32("loginID");
            ViewBag.hotelID = businessID;
            business b = _context.businesses.Where(b => b.ID == businessID).FirstOrDefault();
            var hotelName = _context.businesses.Where(b => b.ID == businessID).Select(b => b.name).FirstOrDefault();

            HttpContext.Session.SetInt32("hotelID11", (int)businessID);
            HttpContext.Session.SetString("hotelName11", hotelName);
            return View();
        }

        [HttpPost] 
        public async Task<string> CountRating()
        {
            var businessID = HttpContext.Session.GetInt32("loginID");
            return getRateForOneHotel(businessID);
        }

        private string getRateForOneHotel(int? businessID)
        {
            var ListroomID = _context.Rooms.Where(r => r.hotelID == businessID).Select(r => r.roomID).ToList();
            var ratingSum = _context.Reviews.Where(r => ListroomID.Contains((int)r.roomID)).Average(r => r.reviewRating);
            string rating = Math.Round(Convert.ToDecimal(ratingSum), 1).ToString();
            return rating;
        }

        [HttpPost]
        public async Task<int> NewBookingNotice()
        {
            var businessID = HttpContext.Session.GetInt32("loginID");
            var CountNewBooking = _context.Bookings.Where(b => b.hotelID == businessID && b.bookingStatus == 1).Count();


            return CountNewBooking;
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
                        await _context.SaveChangesAsync();
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

                return RedirectToActionPreserveMethod(nameof(ShowRoomFromHotel), "Hotels", new { id = InthotelID });
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
        [HttpGet]
        public async Task<IActionResult> FilterB()
        {

            return View();
        }

        //roomtype 新刪修
        [HttpPost]
        public async Task<IEnumerable<roomTypeDTO>> RoomTypeDetails([FromBody]roomTypeDTO RT)
        {
            try
            {
                var hotelID = HttpContext.Session.GetInt32("hotelID11");
                if (RT != null)
                {
                    var findRT = await _context.roomTypes
                        .Where(rt => rt.FK_businessID== hotelID && ( rt.roomType_ID == RT.roomType_ID || rt.roomType1.Contains(RT.roomType1)))
                        .Select(rt => new roomTypeDTO
                        {
                            roomType_ID = rt.roomType_ID,
                            roomType1 = rt.roomType1
                        })
                        .ToListAsync();
                    return findRT;
                }

                var AllRT = await _context.roomTypes.Where(rt=>rt.FK_businessID==hotelID)
                    .Select(rt => new roomTypeDTO
                    {
                        roomType_ID = rt.roomType_ID,
                        roomType1 = rt.roomType1
                    })
                    .ToListAsync();
                return AllRT;
            }
            catch (Exception ex)
            {
                throw new Exception("伺服器錯誤: " + ex.Message);
            }
        }
        [HttpPost]
        public async Task<int> RoomTypeInsert([FromBody]string InsertRT)
        {
            var hotelID = HttpContext.Session.GetInt32("hotelID11");
            if (InsertRT != null)
            {
                //int hotelID = 1004;
                roomType newRT = new roomType
                {
                    FK_businessID = hotelID,
                    roomType1 = InsertRT
                };
                _context.roomTypes.Add(newRT);
                await _context.SaveChangesAsync();
                //var roomType = _context.roomTypes.Where(r => r.FK_businessID == hotelID).ToList();
                return newRT.roomType_ID;
            }
            return 0;
        }
            

        [HttpPut]
        public async Task<roomTypeDTO> RoomTypeEdit(roomTypeDTO roomType)
        {
            var hotelID = HttpContext.Session.GetInt32("hotelID11");
          //  int hotelID = 1004;
            if(roomType.roomType_ID!=null || roomType.roomType_ID > 0)
            {
                var isRoomType = _context.roomTypes.Where(r => r.roomType_ID == roomType.roomType_ID).Select(r=>r.roomType_ID).FirstOrDefault();
                if (isRoomType != null)
                {
                    roomType updateRoomType = new roomType()
                    {
                        roomType_ID = roomType.roomType_ID,
                        roomType1 = roomType.roomType1,
                        FK_businessID = hotelID
                    };
                    _context.roomTypes.Update(updateRoomType);
                    await _context.SaveChangesAsync();
                }
                return roomType;
            }
           
            return null;
        }


        [HttpPost]
        public async Task<roomType> RoomTypeDelete(int roomTypeID)
        {
            var roomType = await _context.Rooms.FindAsync(roomTypeID);
            if (roomType != null)
            {
                _context.Rooms.Remove(roomType);
            }
            await _context.SaveChangesAsync();           
            return null;
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
            //     < td >
            //                @Html.DisplayFor(modelItem => item.roomSpecies)
            //            </ td >
            //            < td >
            //                @Html.DisplayFor(modelItem => item.FK_roomType.roomType1)
            //            </ td >
            //            < td >
            //                @Html.DisplayFor(modelItem => item.roomPrice)
            //            </ td >
            //            < td >
            //                @Html.DisplayFor(modelItem => item.roomDiscount)
            //            </ td >
            //            < td >
            //                @Html.DisplayFor(modelItem => item.roomReserve)
            //            </ td >
            //            < td >
            //                @Html.DisplayFor(modelItem => item.roomDescrep)
            //            </ td >
                var tailstaleContext = _context.Rooms.Include(r => r.hotel).Include(r => r.FK_roomType).Where(r => r.hotelID == id).Select(r=>new RoomDTO
                {
                    roomID=r.roomID,
                    roomSpecies=r.roomSpecies,
                    roomType=r.FK_roomType,
                    roomPrice=r.roomPrice,
                    roomDescrep=r.roomDescrep,
                    roomReserve=r.roomReserve,
                    roomDiscount=r.roomDiscount
                }).ToList();
                //var tailstaleContext = _context.Rooms.Include(r => r.hotel).Where(r=>r.hotelID==hotelID);

                string hotelName = _context.businesses.Where(b => b.ID == id).Select(c => c.name).FirstOrDefault();
                HttpContext.Session.SetInt32("hotelID", id);
                HttpContext.Session.SetString("hotelName", hotelName);
                return View(tailstaleContext);

            }


        }

        //業主訂單管理
        [HttpPost]
        public async Task<JsonResult> BookingData([FromBody]int HotelID)
        {
            if (HotelID != 0) {
                var booking = _context.Bookings.Where(b=>b.hotelID== HotelID).Select(b=>new BookingDTO1
                {
                    BookingID =b.bookingID,
                    KeeperName =b.keeper.name,
                    CheckinDate =b.checkinDate.Value.ToString("yyyy-MM-dd"),
                    CheckoutDate =b.checkoutDate.Value.ToString("yyyy-MM-dd"),
                    BookingStatus =b.bookingStatusNavigation.status_name,
                    BookingTotal =(int)b.bookingAmountTotal,
                    BookingDate =b.bookingDate.Value.ToString("yyyy-MM-dd"),
                }).OrderBy(b=>b.BookingID).ToList();

                // var bookingt = _context.Bookings.Where(b =>  b.hotelID == HotelID).ToList();
                //var options = new JsonSerializerOptions
                //{
                //    WriteIndented = true,
                //    Encoder = System.Text.Encodings.Web.JavaScri  ptEncoder.UnsafeRelaxedJsonEscaping // 允許不轉義的中文字符
                //};
                //var convertBooking = JsonSerializer.Serialize(booking,options);
                return Json(booking);
            }

            return null;
        }

        [HttpPost]
        public async Task<IActionResult> ShowAllBooking(int HotelID)
        {
            ViewBag.HotelID = HttpContext.Session.GetInt32("hotelID11");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ShowBookingDetail([FromForm]int BookingID)
        {
            var loginTYpe = HttpContext.Session.GetInt32("loginType");
            if (loginTYpe == 1)
            {
                ViewBag.HotelID = HttpContext.Session.GetInt32("hotelID11");
                ViewBag.BookingID = BookingID;
                ViewBag.BookingStatus = _context.Bookings.Where(b => b.bookingID == BookingID).Select(b => b.bookingStatus).FirstOrDefault() ?? 0;
                var keeperID = _context.Bookings.Where(b => b.bookingID == BookingID).Select(b => b.keeper_ID).FirstOrDefault() ?? 0;
                DateTime checkinDate = (DateTime)_context.Bookings.Where(b => b.bookingID == BookingID).Select(b => b.checkinDate).FirstOrDefault();
                DateTime checkoutDate = (DateTime)_context.Bookings.Where(b => b.bookingID == BookingID).Select(b => b.checkoutDate).FirstOrDefault();

                //var parsedstartdate = DateTime.Parse(startdate);
                //var parsedenddate = DateTime.Parse(enddate);
                //// 計算日期差
                TimeSpan dateCount = checkoutDate - checkinDate;

                //// 取得天數並轉換為整數
                int totalDays = (int)dateCount.TotalDays;


                ViewBag.KeeperInfo = _context.keepers.Where(k => k.ID == keeperID).FirstOrDefault() ?? null;
                var booking = _context.Bookings.Where(b => b.bookingID == BookingID).Select(b => new BookingDTO1
                {
                    BookingID = b.bookingID,
                    KeeperName = b.keeper.name,
                    CheckinDate = b.checkinDate.Value.ToString("yyyy-MM-dd"),
                    CheckoutDate = b.checkoutDate.Value.ToString("yyyy-MM-dd"),
                    BookingStatus = b.bookingStatusNavigation.status_name,
                    BookingTotal = (int)b.bookingAmountTotal,
                    BookingDate = b.bookingDate.Value.ToString("yyyy-MM-dd"),
                    datecount = totalDays,
                }).FirstOrDefault();

                var bookingdetail = _context.BookingDetails.Where(bd => bd.bookingID == BookingID).Select(bd => new BookingDetailDTO
                {
                    roomPrice = (int)bd.room.roomPrice,
                    bdAmount = bd.bdAmount,
                    bdTotal = bd.bdTotal,
                    roomID = (int)bd.roomID,
                    roomName = bd.room.FK_roomType.roomType1,
                }).ToList();

                var checkinD = _context.CheckinDetails.Where(c => c.bookingID == BookingID).Select(c => new CheckInDTO
                {
                    petID = (int)c.pet_ID,
                    petName = c.pet.name,
                    petType = c.pet.pet_type.species,
                    petBirthDay = DateOnly.Parse(c.pet.birthday.Value.ToString("yyyy-MM-dd")),
                    roomName = c.room.FK_roomType.roomType1,
                    roomID = c.roomID,
                    petChipID = c.pet.chip_ID,
                }).ToList() ?? null;

                var bookingDetailAndCheckins = bookingdetail.Select(bd => new BookingDetailAndCheckin
                {
                    bookingDetail = bd,
                    checkInDTOs = checkinD.Where(c => c.roomID == bd.roomID).ToList()
                }).ToList();


                BBDC getaBooking = new BBDC()
                {
                    book = booking,
                    bookingDetails = bookingDetailAndCheckins,
                };

                ViewBag.Keeper = _context.keepers.Where(k => k.ID == keeperID).FirstOrDefault();


                //return Json(booking);
                return View(getaBooking);
            }
            else if (loginTYpe==2)
            {
                return null;
              //  return RedirectToAction(nameof(ReviewBooking), "Hotels");
              //  return RedirectToActionPreserveMethod(nameof(ShowRoomFromHotel), "Hotels", new { id = InthotelID });
            }
            return RedirectToActionPreserveMethod(nameof(KeeperSearchBookingHistory), "Hotels", new { bookingID = BookingID });

        }

        //變更狀態
        
        [HttpPut]
        public async Task<string> UpdateBookingStatus([FromBody] UpdateStatus update)
        {
            
            var booking = _context.Bookings.FirstOrDefault(b => b.bookingID == update.BookingID);

            if (booking == null)
            {
                return null; 
            }

            
            booking.bookingStatus = update.Status; 

            _context.SaveChanges();
            if (update.Status == 2)
            {
                return "預約成功";
            }
            else if(update.Status == 3)
            {
                return "店家取消";
            }
            else if(update.Status == 4)
            {
                return "顧客取消";
            }
            else
            {
                return null;
            }

             
        }
        [HttpPost]
        public async Task<IActionResult> GetRoomList([FromBody] BookingID getbookingID)
        {
            // int bookingID = 111;
            var roomList = _context.BookingDetails.Where(b => b.bookingID == getbookingID.bookingID).Select(b => new
            {
                roomID = b.roomID,
                roomName = b.room.FK_roomType.roomType1
            }).ToList();
            
            return Json(roomList);
        }

        [HttpGet]
        public IActionResult PostReview([FromQuery]int? bookingID)
        {
           // int bookingID = 111;
            
            return View();
            
        }

        [Route("Hotels/[action]")]
        [HttpPost("CreateReview")]
        public async Task<IActionResult> CreateReview([FromBody] List<ReviewDTO> review)
        {
            var keeperID = HttpContext.Session.GetInt32("loginID");
            //var keeperID = 1002;
            foreach(var r in review)
            {
                Review newreview = new Review()
                {
                    roomID = r.roomID,
                    keeper_ID = keeperID,
                    reviewRating = r.reviewRating,
                    reviewText = r.reviewText,
                    reviewDate = DateTime.Now,
                    bookingID = r.bookingID,
                };
                _context.Reviews.Add(newreview);
            }
           
            await _context.SaveChangesAsync(); 
            return Ok();
        }






            //booking相關

            //查詢
            [HttpGet]
        public async Task<IActionResult> PostDateToSearch()
        {

            return View();
        }
        
        //
        [HttpPost]
        public async Task<List<business>> Filter([FromBody] BusinessFilter businessFilter)
        {
           // BusinessFilter businessFilter = JsonSerializer.Deserialize<BusinessFilter>(a);
           var b = _context.businesses.Where(b => b.type_ID==1 && b.business_status==2 &&
            b.name.Contains(businessFilter.businessName) ||
            b.address.Contains(businessFilter.businessAddress)).ToList();
            return b;
        }

        [HttpGet]
        public async Task<IActionResult> SearchHotels([FromQuery] InputDate iD, int? Cat, int? Dog, string? addressorname)
        {

            var cookie = new
            {
                startdate = iD.startDate.ToString("yyyy-MM-dd"),
                enddate = iD.endDate.ToString("yyyy-MM-dd"),
                cat = Cat == null ? 0 : Cat,
                dog = Dog == null ? 0 : Dog
            };

            SearchCondition setSearchSession = new SearchCondition
            {
                startdate = iD.startDate.ToString("yyyy-MM-dd"),
                enddate = iD.endDate.ToString("yyyy-MM-dd"),
                cat = Cat == null ? 0 : (int)Cat,
                dog = Dog == null ? 0 : (int)Dog

            };




            string convertcookie = JsonSerializer.Serialize(setSearchSession);
            HttpContext.Session.SetString("SearchCondition", convertcookie);
            //SearchCondition jsontos = new SearchCondition();
            //jsontos = JsonSerializer.Deserialize<SearchCondition>(convertcookie);
            //HttpContext.Session.Set<string>("SessionKeyTime", address);
            //HttpContext.Session.Set<SearchCondition>("11", setSearchSession);

            ViewBag.Cookie = cookie;
            var result = await RoomAvailabilityAndRoom(iD, Cat, Dog, addressorname);
            getMyResult = result.ToList();
            var dateCount = (int)ViewBag.totalDays;
            HttpContext.Session.SetInt32("totalDays", dateCount);


            var hotels = result.GroupBy(h => h.hotelID).Select(h => h.Key).ToList();
            // var hotelslist = _context.businesses.Where(h =>( result.GroupBy(h => h.hotelID).Select(h => h.Key)).Contains(h.ID)).ToList();

            var hotelslist = _context.businesses.AsNoTracking().Where(h => hotels.Contains(h.ID)).ToList();

            var myhotelrate = await CountRatingToAllHotel(hotels);

            // var withrate = hotelslist.Join(myhotelrate,h=>(int)h.ID,m=)


            if (Cat != null || Dog != null)
            {
                var resultgroupbyhotel = result.GroupBy(r => r.hotelID).Select(r => new
                {
                    hotelID = r.Key,
                    price = r.Select(r => r.priceTotal).FirstOrDefault(),
                    date = dateCount,
                    onedatePrice = r.Select(r => r.priceTotal).FirstOrDefault() / dateCount,

                });



                var finalresult = hotelslist.Join(resultgroupbyhotel, b => b.ID, r => r.hotelID, (b, r) => new hotelResult
                {
                    businesse = b,
                    roomPrice = r.price,
                    date = r.date,
                    onedatePrice = r.onedatePrice,
                    hotelRate = myhotelrate.FirstOrDefault(hr => hr.hotelID == b.ID)?.rate

                }).ToList();
                //var finalresultWithRate = finalresult.Join(myhotelrate,f=>f.businesse.ID, myhotelrat => m.id(myhotelrate))

                // 現在 finalresultWithRating 包含了每個商業的評分
                // return View(finalresultWithRating);

                //var ff = finalresult.Join(myhotelrate,f=>f.businesse.ID ,m=>m.Key,(final,my))
                return View(finalresult);

            }
            var noCatDog = hotelslist.Select(h => new hotelResult
            {
                businesse = h,
                date = dateCount,
                hotelRate = myhotelrate.FirstOrDefault(hr => hr.hotelID == h.ID)?.rate
            });


            //var hotelslist=_context.businesses.Where(h=>hotels.Contains(h.ID)).ToList();




            // return PartialView("_SearchRoom", finalresult);
            return View(noCatDog);

            //var keeperID = HttpContext.Session.GetInt32("loginID") ;
            //if (keeperID != null)
            //{

            //    HttpContext.Session.SetInt32("KeeperID", (int)keeperID);
               
            //}
            //else
            //{
            //    return RedirectToAction("Index", "LNR");
            //}

        }

        public async Task<IEnumerable<hotelRate>> CountRatingToAllHotel(List<int> hotelList)
        {
            // var businessID = HttpContext.Session.GetInt32("loginID");
            List<hotelRate> hotelRateDict = new List<hotelRate>();
            foreach (var hotel in hotelList) {
                
                var ListroomID = _context.Rooms.Where(r => r.hotelID == hotel).Select(r => r.roomID).ToList();
                var ratingSum = _context.Reviews.Where(r => ListroomID.Contains((int)r.roomID)).Average(r => r.reviewRating);
                string rating = Math.Round(Convert.ToDecimal(ratingSum), 1).ToString();
                hotelRate onehotel = new hotelRate()
                {
                    hotelID = hotel,
                    rate = rating,
                };
                hotelRateDict.Add(onehotel);

            }
            
            return hotelRateDict;
        }


        [HttpGet]
        public async Task<IActionResult> SearchRoom(int ID)
        {
            string convertcookie=HttpContext.Session.GetString("SearchCondition");
            SearchCondition jsontos = JsonSerializer.Deserialize<SearchCondition>(convertcookie);
            //HttpContext.Session.Set<string>("SessionKeyTime", address);
            //HttpContext.Session.Set<SearchCondition>("11", setSearchSession);
            ViewBag.GetCookie = jsontos;
            
            if (HttpContext.Session.GetInt32("loginID") != null)
            {
                var keeperID = HttpContext.Session.GetInt32("loginID");
                HttpContext.Session.SetInt32("KeeperID", (int)keeperID);
            }
            


            // 取得天數並轉換為整數
            int totalDays = (int)HttpContext.Session.GetInt32("totalDays");

            ViewBag.totalDays = totalDays;


            HttpContext.Session.SetInt32("SelectedHotel", ID);
            var useResult = getMyResult.Where(m => m.hotelID == ID).ToList();
            var i=_context.business_img_types.Where(i=>i.FK_business_id == ID).Select(b=>b.ID).ToList();
            var i2 = _context.business_imgs.Where(img=>i.Contains((int)img.img_type_id)).Select(i => i.URL).ToList();
            ViewBag.ImageList= i2;
            
            ViewBag.listCount = useResult.Count;
            ViewBag.thisHotelRate =  getRateForOneHotel(ID);

            var ListroomID = _context.Rooms.Where(r => r.hotelID == ID).Select(r => r.roomID).ToList();
            var review= _context.Reviews.Where(r => ListroomID.Contains((int)r.roomID)).OrderByDescending(r=>r.reviewDate).Select(r=> new ReViewTrans
            {
                keeperName=r.keeper.name,
                reviewRating=(int)r.reviewRating,
                reviewText=r.reviewText,
                reviewDate=r.reviewDate.Value.ToString("yyyy-MM-dd")
            }).ToList();
            ViewBag.reviewList = review;
            // var showHotelReview = _context.Reviews

            //var result = await RoomAvailabilityAndRoom(iD, Cat, Dog, address);
            // var hotels = await result.GroupBy(h => h.hotelID).Select(h => h.Key).ToList();
            var findhotels = _context.businesses.Where(h => ID == h.ID).Select(h => new HotelInfo
             {
                 hotelID = h.ID,
                 hotelname= h.name,
                 hotelAddress= h.address,
                 
             }).FirstOrDefault();

            string hotelInfoForCookie = JsonSerializer.Serialize(findhotels);
            HttpContext.Session.SetString("HotelInfo", hotelInfoForCookie);
            ViewBag.SelectedHotel = findhotels;
            
            // return PartialView("_SearchRoom", finalresult);
            //return View(findhotels);
            return View(useResult);
        }

        //查詢房間圖片
        [HttpPost]
        public async Task<List<string>> SearchImageByRoomID([FromBody] int ID)
        {
            string strID = ID.ToString();
            var i2 = _context.business_imgs.Where(img => img.name.Equals(strID)).Select(i => i.URL).ToList();
            return i2;
        }
       

        //1把取得的房間數量和room合併並轉型 
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
                                   .Where(r => r.hotel.address.Contains(address) || r.hotel.name.Contains(address)).ToListAsync();

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

        //3取得最低房價
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

        //待刪
        private static int getMinRoomPrice(List<RoomSearchResult> getRoom, int theRoomCount)
        {
            int totalPrice = 0;
            foreach (var room in getRoom)
            {
                if (theRoomCount <= 0)
                    break;
                getRoom.GroupBy(g => g.hotelID).Select(g => g);
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
        //2-1取得某物種房間
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
               hotelID = g.hotelID
           })).OrderBy(r => r.roomPrice)
           .ToList();
        }



        //2取得剩餘房間數量 無分物種
        //Bookings/GetBookedRoomIds
        [HttpGet]
        public async Task<IEnumerable<RoomAvailability>> GetBookedRoomIds(DateTime startDate, DateTime endDate)
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
            var bookedRoomsList = await bookedRoom1.ToListAsync();
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

            return availableRooms;
            //return bookedRoom1;

        }

        // /Hotels/FindPet/1001


        [HttpPost, ActionName("FindPet1")]
        public async Task<CheckInDTO> FindPet1([FromBody] int id)
        {
            var a = _context.pets.Where(p => p.pet_ID == id).Select(k => new CheckInDTO
            {
                petID = k.pet_ID,
                petName = k.name,
                petChipID = k.chip_ID,
                petBirthDay = k.birthday.Value == null ? null : k.birthday.Value,
                petType = k.pet_type.species

            }).FirstOrDefault();

            if (a == null)
            {
                return null; // 返回 404 Not Found
            }

            return a;


        }

        //[Route("Hotels/businesslogin/{hotelID:int}")]
        //[HttpGet]
        //[Route("Hotels/FindPet/{id:int}")]
        //public async Task<IActionResult> FindPet(int id)
        //{
        //    var a = _context.pets.Where(p => p.pet_ID == id).Select(k => new CheckInDTO
        //    {
        //        petID = k.pet_ID,
        //        petName = k.name,
        //        petChipID = k.chip_ID,
        //        petBirthDay = k.birthday.Value == null ? null : k.birthday.Value,
        //        petType = k.pet_type.species

        //    }).FirstOrDefault();

        //    if (a == null)
        //    {
        //        return NotFound(); // 返回 404 Not Found
        //    }

        //    return Ok(a);


        //}

       


        [HttpPost]        
        public async Task<IActionResult> CreateCheckinDeTails([FromBody]List<GetChoiceRoom> myChoice)
        {
            //var a1 = obj["roomCount"];
            // var a1 = myChoice.Sum(c=>c.RoomQuantity);


            var a1 = myChoice.Sum(m=>m.RoomQuantity);
            var keeperID = HttpContext.Session.GetInt32("KeeperID");
            string convertcookie = HttpContext.Session.GetString("SearchCondition");
            SearchCondition jsontos = JsonSerializer.Deserialize<SearchCondition>(convertcookie);
            int totalDays = (int)HttpContext.Session.GetInt32("totalDays");
            
            ViewBag.GetCookie = jsontos;

            var a = _context.pets.Where(p => p.keeper_ID == keeperID).Select(k => new CheckInDTO
            {
                petID = k.pet_ID,
                petName = k.name,
                petChipID = k.chip_ID,
                petBirthDay = k.birthday.Value == null ? null : k.birthday.Value,
                petType = k.pet_type.species

            }).ToList();

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping // 允許不轉義的中文字符
            };


            

           
            var room1 = _context.Rooms.Select(r=>new { 
            id=r.roomID,
            typename = r.FK_roomType.roomType1,
            spe=r.roomSpecies,
            price = r.roomPrice,
            hotelid=r.hotelID
            }).ToList();
            

            var result = myChoice
            .Join(room1, r => r.RoomId, rt => rt.id, (r, rt) => new { r, rt })
            .Select(x => new roomInfo
            {
               roomId= x.r.RoomId,
               roomName= x.rt.typename,
               roomPrice=(int)x.rt.price,
               roomQuantity= x.r.RoomQuantity,
               roomPriceTotal= (int)x.rt.price *x.r.RoomQuantity* totalDays,
               roomSpecies =x.rt.spe,
               hotelID=x.rt.hotelid

            }).ToList();

            

            var petSum = result.GroupBy(r => r.roomSpecies).Select(p => new
            {
                petType = p.Key,
                petTypeSum = p.Sum(p=>p.roomQuantity),
            });

            jsontos = new SearchCondition
            {
                startdate = jsontos.startdate,
                enddate = jsontos.enddate,
                cat = petSum.FirstOrDefault(p => p.petType == "貓")?.petTypeSum ?? 0,
                dog = petSum.FirstOrDefault(p => p.petType == "狗")?.petTypeSum ?? 0,
            };


            HttpContext.Session.SetString("finalCondition", JsonSerializer.Serialize(jsontos, options));

            // string tte = JsonSerializer.Serialize(newlist, options);
            string resultToString = JsonSerializer.Serialize(result, options);
            HttpContext.Session.SetString("GetSelectedRoom", resultToString);
            var roomq = 0;
           
            List<RoomListHaveNumber> t = new List<RoomListHaveNumber>();


            //ArrayList<List<int>,List<string>> arrayRoomList = new ArrayList<List<int>, List<string>>();
            string roomnameAddnum = "";
            int rid = 0;
            int c = 0;
           // rid = Convert.ToInt32(room.roomId);
            foreach (var room in result)
            {
                roomq = Convert.ToInt32(room.roomQuantity);
                
                for (int i = 1; i <= roomq; i++)
                {
                    var tt = new RoomListHaveNumber();
                    //ArrayList<ArrayList> arrayRoomList1 = new ArrayList();
                    rid = Convert.ToInt32(room.roomId);
                    c++;
                    roomnameAddnum = $"{room.roomSpecies}-{room.roomName}[{i.ToString()}]";
                    tt.number = c;
                    tt.RoomId = rid;
                    tt.RoomName = roomnameAddnum;
                    t.Add(tt);
                    
                }
            }


            roomListHaveNum = t;
            getPetList = a;
            getroomCount = a1;
         
            return Ok();
        }



        //  private static List<string> getroomList = new List<string>();
        // private static List<RoomListHaveNumber>{get;set;};
        private static List<CheckInDTO> getPetList = new List<CheckInDTO>();
        private static int getroomCount = 0;
        private static List<roomInfo> getroomInfo = new List<roomInfo>();
        //  private static Dictionary<int, string> roomDict = new Dictionary<int, string>();
        public static List<RoomListHaveNumber> roomListHaveNum = new List<RoomListHaveNumber>();


        [HttpGet]
        public async Task<IActionResult> CreateCheckinDeTailShow()
        {
            var keeperID = (int)HttpContext.Session.GetInt32("KeeperID");

            if (keeperID != null || keeperID > 0)
            {
                List<roomInfo> getRoomList = JsonSerializer.Deserialize<List<roomInfo>>(HttpContext.Session.GetString("GetSelectedRoom"));


                var a = roomListHaveNum.ToList();
                var b = getPetList;
                var c = getroomCount;
                var d = getRoomList; //山

                SearchCondition getSelectCondition = JsonSerializer.Deserialize<SearchCondition>(HttpContext.Session.GetString("finalCondition"));

                //// 取得天數並轉換為整數
                int totalDays = (int)HttpContext.Session.GetInt32("totalDays");

                ViewBag.totalDays = totalDays;
                HttpContext.Session.SetInt32("totalDays", totalDays);
                var CatList = b.Where(b => b.petType.Contains("貓")).ToList();
                var DogList = b.Where(b => b.petType == "狗").ToList();
                ViewBag.CatList = new SelectList(CatList, "petID", "petName", "petType");
                ViewBag.DogList = new SelectList(DogList, "petID", "petName", "petType");


                ViewBag.Selected = getSelectCondition;

                var getHotelID = HttpContext.Session.GetInt32("SelectedHotel");
                var gethotel = _context.businesses.Where(h => h.ID == getHotelID).Select(h => new { hotelname = h.name, address = h.address, }).FirstOrDefault();
                ViewBag.HotelInfo = gethotel;
                ViewBag.roomList = a;
                ViewBag.PetList = new SelectList(b, "petID", "petName", "petType");
                ViewBag.roomCount = c;
                ViewBag.roomInfo = d;
                ViewBag.BookingTotal = getRoomList.Sum(r => r.roomPriceTotal);
                return View();
            }
            else
            {
                return RedirectToAction("Index", "LNR");
            }

        }

       


        //List<RoomInfoDTO> Review = new List<RoomInfoDTO>();
        [HttpPost]
        public async Task<IActionResult> CheckinDetailShow([FromBody] List<ReViewCheckinDetail> newRCD)
        {
            var getCondition= HttpContext.Session.GetString("SearchCondition");
            var getSelected = HttpContext.Session.GetString("GetSelectedRoom");
            //取得飼主ID用於查詢payment info
            var keeperID = (int)HttpContext.Session.GetInt32("KeeperID");
            //取得選取的日期及貓狗數量
            SearchCondition convertCondition = JsonSerializer.Deserialize<SearchCondition>(getCondition);
            //取得房間設量統計
            List<roomInfo> getroomList = JsonSerializer.Deserialize<List<roomInfo>>(getSelected);

            var total = getroomList.Sum(r => r.roomPriceTotal);

            //json轉中文解碼
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            // var checkPetIsHaveID = 
            List<ReViewCheckinDetail> myList = new List<ReViewCheckinDetail>();
                foreach (var pet in newRCD)
                {
                    if (pet.petID == null)
                    {
                    pet.petID = (int?)await AddPet(pet);
                    
                    }
                    myList.Add(pet);
                }
            var getReviewCheckinDetails = myList;

            var newa = getReviewCheckinDetails.Select(g => new RCD
            {
                ID=g.petID,
                RID = g.roomId
            });
            string CDToECPay = JsonSerializer.Serialize(newa, options);
            HttpContext.Session.SetString("CDToECPay", CDToECPay);

            string CheckinDetails = JsonSerializer.Serialize(getReviewCheckinDetails, options);
            HttpContext.Session.SetString("CheckinDetails", CheckinDetails);


            List<RoomInfoDTO> ReviewMyBooking = new List<RoomInfoDTO>();
           

            foreach (var room in getroomList)
            {
                var detailforroom=getReviewCheckinDetails.Where(c=>c.roomId==room.roomId).ToList();
                ReviewMyBooking.Add(new RoomInfoDTO
                {
                    roomInfo = room,
                    myReviewBooking = detailforroom
                });
            }

            
            //Review = ReviewMyBooking;
            //return RedirectToAction(nameof(ReviewBooking), "Hotels", new { Review = ReviewMyBooking });
            string reviewtojson = JsonSerializer.Serialize(ReviewMyBooking, options);
            HttpContext.Session.SetString("ReView",reviewtojson);
            BookingIDAndTotal booking = new BookingIDAndTotal()
            {
                ToTalAmount = total,
            };
            
            return Json(new
            {
                redirectUrl = Url.Action("ReviewBooking", "Hotels")
            });
        }

        public async  Task<int> AddPet(ReViewCheckinDetail petInfo)
        {

            var petSpecID = _context.pet_types.Where(pt => petInfo.petType.Contains(pt.species)).Select(pt => pt.ID).FirstOrDefault();
            pet anewpet = new pet()
            {
                pet_type_ID = petSpecID,
                keeper_ID = HttpContext.Session.GetInt32("loginID"),
                name = petInfo.petName,
                chip_ID= petInfo.petChipID,
                birthday= DateOnly.Parse(petInfo.petBirthDay),
            };
            _context.pets.Add(anewpet);
            await _context.SaveChangesAsync();
            
            return anewpet.pet_ID;
        }


        


        [HttpGet]
        public async Task<IActionResult> ReviewBooking()
        {
            //取得飼主ID用於查詢payment info
            var keeperID = HttpContext.Session.GetInt32("KeeperID");
            var keeperName = _context.keepers.Where(k=>k.ID== keeperID).Select(k=>k.name).FirstOrDefault();

            var review =JsonSerializer.Deserialize<List<RoomInfoDTO>>(HttpContext.Session.GetString("ReView"));
           
            var getSelected = HttpContext.Session.GetString("GetSelectedRoom");
           
            SearchCondition getSelectCondition = JsonSerializer.Deserialize<SearchCondition>(HttpContext.Session.GetString("finalCondition"));
           


            // 取得天數並轉換為整數
            int totalDays = (int)HttpContext.Session.GetInt32("totalDays");

            ViewBag.totalDays = totalDays;

            List<roomInfo> getroomList = JsonSerializer.Deserialize<List<roomInfo>>(getSelected);

            //var a = review;
            var AllRoomTotal = getroomList.Sum(p => p.roomPriceTotal);
            var hotelInfo =_context.businesses.Where(b=>b.ID== getroomList.Select(r =>r.hotelID).FirstOrDefault()).FirstOrDefault();
            
            //var keeperID = (int)HttpContext.Session.GetInt32("KeeperID");

            var GetMyCardList = _context.PaymentInfos.Where(p=>p.keeper_ID==keeperID).GroupBy(p=>p.cardNumber).Select(p=>new{cardNumber= p.Key,}).ToList();
            ViewBag.IsCard = GetMyCardList.Count() <=0?0: GetMyCardList.Count();
            ViewBag.myCardList = new SelectList(GetMyCardList, "cardNumber", "cardNumber");
            ViewBag.HotelInfo = hotelInfo;
            ViewBag.KeeperName = keeperName;
            ViewBag.roomInfo = getroomList;
            ViewBag.BookingTotal = AllRoomTotal;
            ViewBag.Selected = getSelectCondition;

            return View(review);
        }

        [HttpPost]
        public async Task<GetCardList> SearchPaymentInfo([FromBody] card mycard)
        {
            var p = _context.PaymentInfos.Where(p => p.cardNumber.Equals(mycard.cardNumber)).Select(p => new GetCardList
            {
                cardName = p.cardholderName,
                cardNumber = p.cardNumber,
                cardExpirationDate = p.expirationDate.ToString()

            }).FirstOrDefault();
            
            if (p!=null)
            {
                return p;
            }
            else
            {
                return null;
            }
           // string strID = ID.ToString();
           // var i2 = _context.business_imgs.Where(img => img.name.Equals(strID)).Select(i => i.URL).ToList();
            
        }
        public async Task<IActionResult> GetECOrderReturnValue(IFormCollection returnform)
        {
            //   PaymentDate
            //  PaymentType
            //RtnCode
            //RtnMsg
            //"[CustomField1, ]"

            var a = returnform;
            Dictionary<string, string> returnDict = new Dictionary<string, string>();
            foreach (var item in a)
            {
                returnDict[item.Key] = item.Value;
            }
           
            if (returnDict["RtnMsg"].Equals("Succeeded") && (returnDict.TryGetValue("CustomField1", out string customField1Value)))
            {
                int bookingID = Int32.Parse(customField1Value);
                return RedirectToActionPreserveMethod("SaveBooking", "Hotels", new { bookingID = bookingID });
            }
                
            else
            {
                // 處理 CustomField1 不存在的情況
                // 可以選擇返回錯誤或進行其他處理
                return BadRequest("CustomField1 is missing.");
            }
           
            
           
           // return returnDict;
        }

        [HttpPost]
        public async Task<string> CreateANewBooking()
        {
            var finalCondition = JsonSerializer.Deserialize<SearchCondition>(HttpContext.Session.GetString("finalCondition"));
            var getSelected = JsonSerializer.Deserialize<List<roomInfo>>(HttpContext.Session.GetString("GetSelectedRoom"));
            var checkinDetail = JsonSerializer.Deserialize<List<ReViewCheckinDetail>>(HttpContext.Session.GetString("CheckinDetails"));
            var hotelID = getSelected.Select(g => g.hotelID).FirstOrDefault();
            var keepID = HttpContext.Session.GetInt32("KeeperID");
            var bookingAmountTotal = getSelected.Sum(g => g.roomPriceTotal);

           
            Booking mybooking = new Booking
            {
                keeper_ID = keepID,
                hotelID = hotelID,
                checkinDate = DateTime.Parse(finalCondition.startdate),
                checkoutDate = DateTime.Parse(finalCondition.enddate),
                bookingAmountTotal = bookingAmountTotal,
                bookingStatus = 18,
                bookingDate = DateTime.Now
            };
            _context.Bookings.Add(mybooking);
            await _context.SaveChangesAsync();
            foreach (var bd in getSelected)
            {
                BookingDetail mybookingDetail = new BookingDetail
                {
                    bookingID = mybooking.bookingID,
                    roomID = bd.roomId,
                    bdAmount = bd.roomQuantity,
                    bdTotal = bd.roomPriceTotal,
                };
                _context.BookingDetails.Add(mybookingDetail);

            }
            foreach (var cd in checkinDetail)
            {
                CheckinDetail mycheckinDetail = new CheckinDetail
                {
                    bookingID = mybooking.bookingID,
                    roomID = cd.roomId,
                    pet_ID = cd.petID,
                };
                _context.CheckinDetails.Add(mycheckinDetail);
            }



            await _context.SaveChangesAsync();
            return mybooking.bookingID.ToString();
        }
      
        [HttpPost]
        public async Task<IActionResult> SaveBooking(int bookingID)
        {
            
            var mybooking = _context.Bookings.FirstOrDefault(b => b.bookingID == bookingID);
            mybooking.bookingStatus = 1;
            _context.Bookings.Update(mybooking);
            await _context.SaveChangesAsync();

            BookingAndCheckinDTO bc = new BookingAndCheckinDTO
            {
                booking = mybooking,
                bookingDetails = _context.BookingDetails.Where(b => b.bookingID == mybooking.bookingID).Include(bd => bd.room).Select(bd => new BookingDetailDTO
                {
                    roomPrice = (int)bd.room.roomPrice,
                    bdAmount = bd.bdAmount,
                    bdTotal = bd.bdTotal,
                    roomID = (int)bd.roomID,
                    roomName = bd.room.FK_roomType.roomType1,
                }).ToList(),
                checkinDetails = _context.CheckinDetails.Where(b => b.bookingID == mybooking.bookingID).Include(bd => bd.room).Select(c => new CheckInDTO
                {
                    roomName = c.room.FK_roomType.roomType1,
                    petID = (int)c.pet_ID,
                    petName = c.pet.name,
                    petType = c.pet.pet_type.species,
                    petBirthDay = c.pet.birthday,

                }).ToList(),
            };
            
            ViewBag.hotelName = _context.businesses.Where(h => h.ID == mybooking.hotelID).Select(h => h.name).FirstOrDefault();





            return View(bc);

        }
        
        [HttpPost]
        public async Task<IActionResult> KeeperSearchBookingHistory([FromForm]int bookingID)
        {
           var keeperID = HttpContext.Session.GetInt32("loginID");
           // var keeperID = 1002;
            var getBookingHistory = _context.Bookings.Where(book => book.keeper_ID == keeperID && book.bookingID == bookingID).FirstOrDefault();
           
            if (getBookingHistory != null)
            {
                var getHotelName = getBookingHistory.hotel.name;
                var getCard = getBookingHistory.PaymentInfos.Select(p => p.cardNumber).FirstOrDefault();
                var mybookingID = getBookingHistory.bookingID;
                var keeperName = getBookingHistory.keeper.name;
                var bookingstatus=getBookingHistory.bookingStatusNavigation.status_name;
                var isRate = false;
                var nowdate = DateTime.Now;
                var ReView = _context.Reviews.FirstOrDefault(b => b.bookingID == bookingID);
                if (nowdate>= getBookingHistory.checkoutDate && bookingstatus.Equals("預約成功") )
                {
                    isRate = true;
                };
                ViewBag.isRate = isRate;
                
                var isReView = false;
                if (ReView != null ) { 
                    isReView = true;
                
                }
                ViewBag.isReView= isReView;
                BookingAndCheckinDTO bc = new BookingAndCheckinDTO
                {
                    booking = getBookingHistory,
                    bookingDetails = _context.BookingDetails.Where(b => b.bookingID == mybookingID).Include(bd => bd.room).Select(bd => new BookingDetailDTO
                    {
                        roomPrice = (int)bd.room.roomPrice,
                        bdAmount = bd.bdAmount,
                        bdTotal = bd.bdTotal,
                        roomID = (int)bd.roomID,
                        roomName = bd.room.FK_roomType.roomType1,
                    }).ToList(),
                    checkinDetails = _context.CheckinDetails.Where(b => b.bookingID == mybookingID).Include(bd => bd.room).Select(c => new CheckInDTO
                    {
                        roomName = c.room.FK_roomType.roomType1,
                        petID = (int)c.pet_ID,
                        petName = c.pet.name,
                        petType = c.pet.pet_type.species,
                        petBirthDay = c.pet.birthday,

                    }).ToList(),
                };

                ViewBag.hotelName = getHotelName;
                ViewBag.cardNumber = getCard;
                ViewBag.keeperName = keeperName;
                ViewBag.bookingstatus = bookingstatus;
                return View(bc);



            }

            return null;

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

        private class RCD
        {
            public int? ID { get; set; }
            public int RID { get; set; }
        }
    }

  
}
