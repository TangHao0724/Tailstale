namespace Tailstale.Hospital_DTO
{
    public class HospitalInfo_DTO
    {
        public int businessId { get; set; }//from business
        public string clinicName { get; set; }//from business
        public string clinicPhone { get; set; }//from business

        public string clincAddress { get; set; }//from business
        public string description { get; set; }//from business

        public string photoURL { get; set; }//from business
        
    }

    public class outpatientClinicInfo
    {
        public DateOnly date { get; set; }//from daily_outpatient_clinic_schedule

        public string outpatientClinicName { get; set; }//from outpatient_clinic
        public string vetName { get; set; }//from vet_Informations
        public string timeslotName { get; set; }//from outpatient_clinic_timeslot
        public TimeOnly startAt { get; set; }//from outpatient_clinic_timeslot

        public TimeOnly endAt { get; set; }//from outpatient_clinic_timeslot   
    }
}
