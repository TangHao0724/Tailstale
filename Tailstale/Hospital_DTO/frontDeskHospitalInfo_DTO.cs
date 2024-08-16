namespace Tailstale.Hospital_DTO
{
    public class frontDeskHospitalCard_DTO
    {
        public int businessId { get; set; }//from business
        public string hospitalName { get; set; }//from business
        public string hospitalPhone { get; set; }//from business

        public string hospitalAddress { get; set; }//from business
        public string photoURL { get; set; }//from business
        
    }
    public class frontDeskHospitalInfo_DTO
    {
        public int businessId { get; set; }//from business
        public string hospitalName { get; set; }//from business
        public string hospitalPhone { get; set; }//from business

        public string hospitalAddress { get; set; }//from business
        public string description { get; set; }//from business
        public string photoURL { get; set; }//from business

    }

    public class OutpatientClinicInfo_DTO
    {
        public DateOnly date { get; set; }//from daily_outpatient_clinic_schedule

        public string outpatientClinicName { get; set; }//from outpatient_clinic
        public string vetName { get; set; }//from vet_Informations
        public string timeslotName { get; set; }//from outpatient_clinic_timeslot
        public TimeOnly startAt { get; set; }//from outpatient_clinic_timeslot

        public TimeOnly endAt { get; set; }//from outpatient_clinic_timeslot   
    }
}
