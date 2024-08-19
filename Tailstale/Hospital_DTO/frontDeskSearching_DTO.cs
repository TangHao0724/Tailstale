using Tailstale.Models;

namespace Tailstale.Hospital_DTO
{
    public class frontDeskSearching_DTO
    {
        public IEnumerable<frontDeskSearchingResult_DTO> frontDeskSearchingResult;
        public IEnumerable<business> hospitalCardSearchingResult;
    }

    public class frontDeskSearchingCriteria_DTO
    {
        public string? region_front { get; set; }
        public string? address_front { get; set; }
        public DateOnly? startDate_front { get; set; }
        public DateOnly? endDate_front { get; set; }
        public string? timeSlotName_front { get; set; }
        public string? opcName_front { get; set; }
        public string? vetName_front { get; set; }
        public string? clinicName_front { get; set; }        
    }

    public class frontDeskSearchingResult_DTO
    {
        public DateOnly date { get; set; }//from daily_outpatient_clinic_schedule
        
        public string outpatientClinicName { get; set; }//from outpatient_clinic
        public int maxPatients { get; set; }//from outpatient_clinic
        public string vetName { get; set; }
        public string timeslotName { get; set; }//from outpatient_clinic_timeslot
        public TimeOnly startAt { get; set; }//from outpatient_clinic_timeslot

        public TimeOnly endAt { get; set; }//from outpatient_clinic_timeslot
        public int outpatientClinicScheduleId { get; set; }//from daily_outpatient_clinic_schedule
        public string clinicName { get; set; }//from business
        public string clinicPhone { get; set; }//from business

        public string clincAddress { get; set; }//from business        
        public int appointmentCount { get; set; }
        public int businessID { get; set; }
    }

    public class frontDeskSearchingResultID_DTO
    { 
        public int businessID { get; set; }
    }
}
