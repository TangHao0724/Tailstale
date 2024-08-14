namespace Tailstale.Hospital_DTO
{
    
    public class frontDeskDailyOutPatientClinicSchedule_DTO
    {
        public int dailyOutpatientClinicScheduleID { get; set; }//from daily_outpatient_clinic_schedule
        public DateOnly date { get; set; }//from daily_outpatient_clinic_schedule
        public string OutpatientClinicName { get; set; }//from outpatient_clinic
        public string vetName { get; set; }//from vet_informations

        public string outpatientClinicTimeslotName { get; set; }//from outpatient_clinic_timeslot

        public TimeOnly startAt { get; set; }//from outpatient_clinic_timeslot

        public TimeOnly endAt { get; set; }//from outpatient_clinic_timeslot

        public bool dailyOutpatientClinicScheduleStatus { get; set; }//from daily_outpatient_clinic_schedule
    }
}
