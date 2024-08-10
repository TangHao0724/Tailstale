using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tailstale.Hospital_DTO;
using Tailstale.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Tailstale.Controllers
{
    public class Hospital_FrontDeskController : Controller
    {
        private readonly TailstaleContext _context;

        public Hospital_FrontDeskController(TailstaleContext context)
        {
            _context = context;
        }
        public IActionResult Index()//搜尋頁面
        {
            return View();
        }

        //GET:Hospital_FrontDesk/showSearchingResult
        //https://localhost:7112/hospital_FrontDesk/showSearchingResult
        [HttpPost]
        public async Task<IEnumerable<frontDeskSearchingResult_DTO>> showSearchingResult([FromBody]frontDeskSearchingCriteria_DTO criteria)
        {
            var searchingCriteria = criteria;
            var searchingResult = new List<frontDeskSearchingResult_DTO>();
            if (searchingCriteria == null)
            {
                return null;
            }
            else
            {
                string address = criteria.Address;
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
                                            where docs.date >= today
                                                && docs.date <= oneMonthLater                                                
                                                && b.address.Contains(address)
                                                && opct.outpatient_clinic_timeslot_name.Contains(timeSlotName)
                                                && opc.outpatient_clinic_name.Contains(opcNameSearch)
                                                && vInfo.vet_name.Contains(vetName)
                                                && b.name.Contains(clinicName)
                                            select new frontDeskSearchingResult_DTO
                                            {
                                                date = (DateOnly)docs.date,
                                                clinicName = b.name,
                                                outpatientClinicName = opc.outpatient_clinic_name,
                                                timeslotName = opct.outpatient_clinic_timeslot_name,
                                                vetName = vInfo.vet_name,
                                                startAt = (TimeOnly)opct.startat,
                                                endAt = (TimeOnly)opct.endat,
                                                businessId=b.ID,
                                                clinicPhone = b.phone,
                                                clincAddress = b.address
                                            }).ToListAsync();

                if (startDate != null && endDate != null)
                {
                    searchingResult = await (from docs in _context.daily_outpatient_clinic_schedules
                                             join opc in _context.outpatient_clinics on docs.outpatient_clinic_ID equals opc.outpatient_clinic_ID
                                             join opct in _context.outpatient_clinic_timeslots on opc.outpatient_clinic_timeslot_ID equals opct.outpatient_clinic_timeslot_ID
                                             join vInfo in _context.vet_informations on opc.vet_ID equals vInfo.vet_ID
                                             join b in _context.businesses on vInfo.business_ID equals b.ID
                                             where docs.date >= startDate && docs.date <= endDate
                                             select new frontDeskSearchingResult_DTO
                                             {
                                                 date = (DateOnly)docs.date,
                                                 clinicName = b.name,
                                                 outpatientClinicName = opc.outpatient_clinic_name,
                                                 timeslotName = opct.outpatient_clinic_timeslot_name,
                                                 vetName = vInfo.vet_name,
                                                 startAt = (TimeOnly)opct.startat,
                                                 endAt = (TimeOnly)opct.endat,
                                                 clinicPhone = b.phone,
                                                 clincAddress = b.address
                                             }).ToListAsync();
                }
                else
                {

                }               
                return searchingResult;
            }
        }

        [HttpGet]
        public IActionResult HospitalIndex(HospitalInfo_DTO hospitalInfo, outpatientClinicInfo opcInfo)//院所首頁
        {
            
            return View();
        }

        public async Task<IEnumerable<HospitalInfo_DTO>> showVetInfo([FromBody] )
        {
            return null;
        }


    }

    
}
