using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Linq;
using Tailstale.Filter;
using Tailstale.Hospital_DTO;
using Tailstale.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Tailstale.Controllers
{
    //[IsKeeperFilter]
    public class Hospital_FrontDeskController : Controller
    {
        private readonly TailstaleContext _context;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public Hospital_FrontDeskController(TailstaleContext context, IConfiguration configuration, IWebHostEnvironment environment)
        {
            _context = context;
            _configuration = configuration;
            _environment = environment;
        }
        public IActionResult Index()//搜尋頁面
        {
            return View();
        }

        //POST:Hospital_FrontDesk/showSearchingResult
        //https://localhost:7112/Hospital_FrontDesk/showSearchingResult
        [HttpPost]
        public async Task<IEnumerable<frontDeskSearchingResult_DTO>> showSearchingResult([FromBody] frontDeskSearchingCriteria_DTO criteria)//搜尋門診結果
        {
            var searchingResult = new List<frontDeskSearchingResult_DTO>();
            if (criteria.region_front == "" && criteria.address_front == "" && criteria.startDate_front == null && criteria.endDate_front == null && criteria.timeSlotName_front == "" && criteria.opcName_front == "" && criteria.vetName_front == "" && criteria.clinicName_front == "")
            {
                return null;
            }
            else
            {
                string region = criteria.region_front;
                string address = criteria.address_front;
                DateOnly today = DateOnly.FromDateTime(DateTime.Now);
                DateOnly oneMonthLater = today.AddMonths(1);
                DateOnly? startDate = criteria.startDate_front;
                DateOnly? endDate = criteria.endDate_front;
                string timeSlotName = criteria.timeSlotName_front;
                string opcNameSearch = criteria.opcName_front;
                string vetName = criteria.vetName_front;
                string clinicName = criteria.clinicName_front;


                searchingResult = await (from docs in _context.daily_outpatient_clinic_schedules
                                         join opc in _context.outpatient_clinics on docs.outpatient_clinic_ID equals opc.outpatient_clinic_ID
                                         join opct in _context.outpatient_clinic_timeslots on opc.outpatient_clinic_timeslot_ID equals opct.outpatient_clinic_timeslot_ID
                                         join vInfo in _context.vet_informations on opc.vet_ID equals vInfo.vet_ID
                                         join b in _context.businesses on vInfo.business_ID equals b.ID
                                         join a in _context.Appointments on docs.outpatient_clinic_ID equals a.Appointment_ID
                                         where docs.date >= today
                                             && docs.date <= oneMonthLater
                                             && b.address.Contains(region)
                                             && b.address.Contains(address)
                                             && opct.outpatient_clinic_timeslot_name.Contains(timeSlotName)
                                             && opc.outpatient_clinic_name.Contains(opcNameSearch)
                                             && vInfo.vet_name.Contains(vetName)
                                             && b.name.Contains(clinicName)
                                         select new frontDeskSearchingResult_DTO
                                         {
                                             businessID=b.ID,
                                             date = (DateOnly)docs.date,
                                             clinicName = b.name,
                                             outpatientClinicName = opc.outpatient_clinic_name,
                                             timeslotName = opct.outpatient_clinic_timeslot_name,
                                             vetName = vInfo.vet_name,
                                             startAt = (TimeOnly)opct.startat,
                                             endAt = (TimeOnly)opct.endat,
                                             outpatientClinicScheduleId = docs.daily_outpatient_clinic_schedule_ID,
                                             clinicPhone = b.phone,
                                             clincAddress = b.address,
                                             maxPatients = opc.max_patients,
                                             appointmentCount = _context.Appointments.Where(a => a.daily_outpatient_clinic_schedule_ID == docs.daily_outpatient_clinic_schedule_ID).Count()
                                         }).ToListAsync();

                if (startDate.HasValue && endDate.HasValue)
                {
                    searchingResult = searchingResult
                        .Where(s => s.date >= startDate.Value && s.date <= endDate.Value)
                        .ToList();
                }
                else
                {
                    searchingResult = searchingResult;
                }

                //var hospitalID = searchingResult.Select(s=>s.businessID).Distinct().ToList();
                //var hospitals = await (from b in _context.businesses
                //                            where b.type_ID==3
                //                            && hospitalID.Contains(b.ID)
                //                            select new business
                //                            { 
                //                             ID = b.ID,
                //                             name=b.name,
                //                             address=b.address,
                //                             phone=b.phone,
                //                             photo_url=b.photo_url,
                //                            }).ToListAsync();


                //frontDeskSearching_DTO frontDeskSearching = new frontDeskSearching_DTO
                //{
                //    frontDeskSearchingResult = searchingResult.Select(s => s).ToList(),
                //    hospitalCardSearchingResult = hospitals.Select(s => s).ToList()
                //};
                return searchingResult;                
            }
        }

        //GET:Hospital_FrontDesk/loadHospitalCards
        [HttpPost]
        public async Task<IEnumerable<frontDeskHospitalCard_DTO>> loadHospitalCards([FromBody] frontDeskSearchingCriteria_DTO criteria)//首頁院所資料卡片
        
        {
            var imageURL = $"{_configuration["BaseAddress"]}/lib/HospitalImages";
            if (criteria.region_front == "" && criteria.address_front == "" && criteria.startDate_front == null && criteria.endDate_front == null && criteria.timeSlotName_front == "" && criteria.opcName_front == "" && criteria.vetName_front == "" && criteria.clinicName_front == "")
            {
                var hospitals = await (from b in _context.businesses
                                       where b.type_ID == 3
                                       select new frontDeskHospitalCard_DTO
                                       {
                                           businessId = b.ID,
                                           hospitalName = b.name,
                                           hospitalAddress = b.address,
                                           hospitalPhone = b.phone,
                                           photoURL = b.photo_url
                                       }).ToListAsync();
                if (hospitals == null)
                {
                    return null;
                }
                else
                {
                    foreach (var h in hospitals)
                    {
                        if (checkImage(h.photoURL))
                        {
                            h.photoURL = $"{imageURL}/{h.photoURL}";
                        }
                        else
                        {
                            h.photoURL = $"{imageURL}/No_Image.jpg";
                        }
                    }
                }
                return hospitals;
            }
            else
            {
                string region = criteria.region_front;
                string address = criteria.address_front;
                DateOnly today = DateOnly.FromDateTime(DateTime.Now);
                DateOnly oneMonthLater = today.AddMonths(1);
                DateOnly? startDate = criteria.startDate_front;
                DateOnly? endDate = criteria.endDate_front;
                string timeSlotName = criteria.timeSlotName_front;
                string opcNameSearch = criteria.opcName_front;
                string vetName = criteria.vetName_front;
                string clinicName = criteria.clinicName_front;                

                 var searchingResult = await (from docs in _context.daily_outpatient_clinic_schedules
                                         join opc in _context.outpatient_clinics on docs.outpatient_clinic_ID equals opc.outpatient_clinic_ID
                                         join opct in _context.outpatient_clinic_timeslots on opc.outpatient_clinic_timeslot_ID equals opct.outpatient_clinic_timeslot_ID
                                         join vInfo in _context.vet_informations on opc.vet_ID equals vInfo.vet_ID
                                         join b in _context.businesses on vInfo.business_ID equals b.ID
                                         join a in _context.Appointments on docs.outpatient_clinic_ID equals a.Appointment_ID
                                         where docs.date >= today
                                             && docs.date <= oneMonthLater
                                             && b.address.Contains(region)
                                             && b.address.Contains(address)
                                             && opct.outpatient_clinic_timeslot_name.Contains(timeSlotName)
                                             && opc.outpatient_clinic_name.Contains(opcNameSearch)
                                             && vInfo.vet_name.Contains(vetName)
                                             && b.name.Contains(clinicName)
                                         select new frontDeskSearchingResult_DTO
                                         {
                                             businessID = b.ID,
                                             date = (DateOnly)docs.date,
                                             clinicName = b.name,
                                             outpatientClinicName = opc.outpatient_clinic_name,
                                             timeslotName = opct.outpatient_clinic_timeslot_name,
                                             vetName = vInfo.vet_name,
                                             startAt = (TimeOnly)opct.startat,
                                             endAt = (TimeOnly)opct.endat,
                                             outpatientClinicScheduleId = docs.daily_outpatient_clinic_schedule_ID,
                                             clinicPhone = b.phone,
                                             clincAddress = b.address,
                                             maxPatients = opc.max_patients,
                                             appointmentCount = _context.Appointments.Where(a => a.daily_outpatient_clinic_schedule_ID == docs.daily_outpatient_clinic_schedule_ID).Count()
                                         }).ToListAsync();

                if (startDate.HasValue && endDate.HasValue)
                {
                    searchingResult = searchingResult
                        .Where(s => s.date >= startDate.Value && s.date <= endDate.Value)
                        .ToList();
                }
                else
                {
                    searchingResult = searchingResult;
                }

                var hospitalID = searchingResult.Select(s=>s.businessID).Distinct().ToList();
                IEnumerable<frontDeskHospitalCard_DTO> hospitals;
                if (hospitalID.Count()== 0)
                {
                   hospitals = await (from b in _context.businesses
                                       where b.type_ID == 3
                                       select new frontDeskHospitalCard_DTO
                                       {
                                           businessId = b.ID,
                                           hospitalName = b.name,
                                           hospitalAddress = b.address,
                                           hospitalPhone = b.phone,
                                           photoURL = b.photo_url
                                       }).ToListAsync();
                }
                else 
                {
                    hospitals = await (from b in _context.businesses
                                           where b.type_ID == 3 && hospitalID.Contains(b.ID)
                                           select new frontDeskHospitalCard_DTO
                                           {
                                               businessId = b.ID,
                                               hospitalName = b.name,
                                               hospitalAddress = b.address,
                                               hospitalPhone = b.phone,
                                               photoURL = b.photo_url
                                           }).ToListAsync();
                }
                
                foreach (var h in hospitals)
                {
                    if (checkImage(h.photoURL))
                    {
                        h.photoURL = $"{imageURL}/{h.photoURL}";
                    }
                    else
                    {
                        h.photoURL = $"{imageURL}/No_Image.png";
                    }
                }
                
                return hospitals;
            }
        }

        public async Task<IActionResult> HospitalIndex(int businessID)//院所主頁
        {
            //selectedBusinessID=businessID;
            var imageURL = $"{_configuration["BaseAddress"]}/lib/HospitalImages";
            var HospitalInfo = await (from b in _context.businesses
                                      where b.ID == businessID
                                      select new frontDeskHospitalInfo_DTO
                                      {
                                          businessId = b.ID,
                                          hospitalName = b.name,
                                          hospitalAddress = b.address,
                                          hospitalPhone = b.phone,
                                          description = b.description,
                                          photoURL = b.photo_url,
                                      }).SingleOrDefaultAsync();
            if (HospitalInfo == null)
            {
                return null;
            }
            else
            {
                if (checkImage(HospitalInfo.photoURL))
                {
                    HospitalInfo.photoURL = $"{imageURL}/{HospitalInfo.photoURL}";
                }
                else
                {
                    HospitalInfo.photoURL = $"{imageURL}/No_Image.png";
                }

            }
            return View(HospitalInfo);
        }


        //GET:Hospital_FrontDesk/showVetInfos        
        [HttpGet]//取得院所首頁醫生資訊
        public async Task<IEnumerable<frontDeskVetInfo_DTO>> showVetInfos(int businessID)
        {
            var imageURL = $"{_configuration["BaseAddress"]}/lib/HospitalImages";
            var vetInfos = await (from v in _context.vet_informations
                                  join b in _context.business_imgs
                                  on v.business_img_ID equals b.ID
                                  join d in _context.departments
                                  on v.department_ID equals d.department_ID
                                  where v.business_ID == businessID
                                  select new frontDeskVetInfo_DTO
                                  {
                                      vetID=v.vet_ID,
                                      vetName = v.vet_name,
                                      departmentName=d.department_name,
                                      profile = v.profile,
                                      vetImageURL = b.URL,
                                  }).ToListAsync();
            if (vetInfos == null)
            {
                return null;
            }
            else
            {
                foreach (var v in vetInfos)
                {
                    if (checkImage(v.vetImageURL))
                    {
                        v.vetImageURL = $"{imageURL}/{v.vetImageURL}";
                    }
                    else
                    {
                        v.vetImageURL = $"{imageURL}/No_Image.jpg";
                    }
                }
            }
            return vetInfos;
        }
        private bool checkImage(string photoURL)
        {
            if (!string.IsNullOrEmpty(photoURL))//判斷圖檔是否存在
            {
                //取得當前目錄及圖片存放路徑，去掉開頭的'/'以防止路徑不正確
                var filePath = Path.Combine(_environment.ContentRootPath, "wwwroot/lib/HospitalImages", photoURL.TrimStart('/'));
                //確認圖片是否能顯示
                if (System.IO.File.Exists(filePath))
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public async Task<IActionResult> HospitalSchedule(int businessID)//搜尋頁面
        {
            return View();
        }

        //GET:Hospital_FrontDesk/getSchedules/businessID
        [HttpGet]//取得當月及下個月門診資訊
        public async Task<IEnumerable<frontDeskDailyOutPatientClinicSchedule_DTO>> getSchedules(int businessID)
        {
            int currentYear = DateTime.Now.Year;
            int currentMonth = DateTime.Now.Month;
            // 計算下個月份值，如果當月份是12月，則下個月為1月
            int nextMonth = (currentMonth == 12) ? 1 : currentMonth + 1;
            // 計算下個月的年份值，如果當月份是12月，下個月為下一年的1月，否則為當年
            int nextMonthYear = (currentMonth == 12) ? currentYear + 1 : currentYear;

            var schedules = await (from docs in _context.daily_outpatient_clinic_schedules
                                   join opc in _context.outpatient_clinics
                                   on docs.outpatient_clinic_ID equals opc.outpatient_clinic_ID
                                   join opct in _context.outpatient_clinic_timeslots
                                   on opc.outpatient_clinic_timeslot_ID equals opct.outpatient_clinic_timeslot_ID
                                   join vInfo in _context.vet_informations
                                   on opc.vet_ID equals vInfo.vet_ID
                                   where vInfo.business_ID == businessID
                                   && ((docs.date.Value.Year == currentYear && docs.date.Value.Month == currentMonth)
                                   || (docs.date.Value.Year == nextMonthYear && docs.date.Value.Month == nextMonth))
                                   orderby opct.startat
                                   select new frontDeskDailyOutPatientClinicSchedule_DTO
                                   {
                                       dailyOutpatientClinicScheduleID = docs.daily_outpatient_clinic_schedule_ID,
                                       date = (DateOnly)docs.date,
                                       outpatientClinicName = opc.outpatient_clinic_name,
                                       vetName = vInfo.vet_name,
                                       outpatientClinicTimeslotName = opct.outpatient_clinic_timeslot_name,
                                       startAt = (TimeOnly)opct.startat,
                                       endAt = (TimeOnly)opct.endat,
                                       dailyOutpatientClinicScheduleStatus = docs.daily_outpatient_clinic_schedule_status,
                                       maxPatients = opc.max_patients,
                                       appointmentCount = _context.Appointments.Where(a => a.daily_outpatient_clinic_schedule_ID == docs.daily_outpatient_clinic_schedule_ID).Count()
                                   }).ToListAsync();

            if (schedules == null)
            {
                return null;
            }
            return schedules;
        }


        //Post:Hospital_FrontDesk/fetchAppointmentInfo

        [HttpGet]//取得飼主及門診資訊顯示在預約確認表單
        public async Task<AppointmentComfrim_DTO> fetchAppointmentInfo(int dailyOutpatientClinicScheduleID)
        {
            //int keeperID = (int)HttpContext.Session.GetInt32("loginID");
            int keeperID = 1001;
            var keeperInfo = await (from k in _context.keepers
                                    where k.ID == keeperID
                                    select new KeeperInfo_DTO
                                    {
                                        keeperID = k.ID,
                                        keeperName = k.name,
                                    }).SingleOrDefaultAsync();
            var petInfo = await (from p in _context.pets
                                 where p.keeper_ID==keeperID
                                 select new PetInfo_DTO
                                 { 
                                  petID=p.pet_ID,
                                  petName = p.name,
                                 }).ToListAsync();

            var newAppointment = await (from docs in _context.daily_outpatient_clinic_schedules
                                        join opc in _context.outpatient_clinics
                                        on docs.outpatient_clinic_ID equals opc.outpatient_clinic_ID
                                        join vInfo in _context.vet_informations
                                        on opc.vet_ID equals vInfo.vet_ID
                                        join opct in _context.outpatient_clinic_timeslots
                                        on opc.outpatient_clinic_timeslot_ID equals opct.outpatient_clinic_timeslot_ID
                                        join b in _context.businesses
                                        on vInfo.business_ID equals b.ID
                                        where docs.daily_outpatient_clinic_schedule_ID == dailyOutpatientClinicScheduleID
                                        select new AppointmentComfrim_DTO
                                        {
                                            dailyOutpatientClinicScheduleID = docs.daily_outpatient_clinic_schedule_ID,
                                            date = (DateOnly)docs.date,
                                            outpatientClinicName = opc.outpatient_clinic_name,
                                            clinicName = b.name,
                                            vetName = vInfo.vet_name,
                                            outpatientClinicTimeslotName = opct.outpatient_clinic_timeslot_name,
                                            startAt = (TimeOnly)opct.startat,
                                            endAt = (TimeOnly)opct.endat,
                                            keeperID = keeperInfo.keeperID,
                                            keeperName = keeperInfo.keeperName,
                                            petInfos= petInfo
                                        }).SingleOrDefaultAsync();
            if (newAppointment == null)
            {
                return null;
            }
            return newAppointment;
        }

        //GET:Hospital_FrontDesk/fetchPetInfo
        [HttpGet]//取飼主的寵物資訊顯示在下拉選單
        public async Task<IEnumerable<PetInfo_DTO>> fetchPetInfo()
        {
            int keeperID = (int)HttpContext.Session.GetInt32("loginID");
            var petInfos = await (from p in _context.pets
                                  where p.keeper_ID == keeperID
                                   select new PetInfo_DTO
                                   { 
                                    petID=p.pet_ID,
                                    petName=p.name,
                                   }).ToListAsync();
            return petInfos;
        }

        //POST:Hospital_FrontDesk/newAppointment
        [HttpPost]//送出預約請求
        public async Task<bool> newAppointment([FromBody] NewAppointment_DTO appointment)
        {
            Appointment newAppointment = new Appointment
            {
                daily_outpatient_clinic_schedule_ID= appointment.dailyOutpatientClinicScheduleID,
                keeper_ID= appointment.keeperID,
                pet_ID= appointment.petID,
                Appointment_status=13
            };
            try
            {
                _context.Appointments.Add(newAppointment);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
             return false;
            }
            return true;
        }

    }


}
