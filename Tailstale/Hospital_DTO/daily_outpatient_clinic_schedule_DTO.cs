namespace Tailstale.Hospital_DTO
{
    public class daily_outpatient_clinic_schedule_DTO
    {
        public int daily_outpatient_clinic_schedule_ID { get; set; }//from daily_outpatient_clinic_schedule
        public DateOnly? date { get; set; }//from daily_outpatient_clinic_schedule
        public string outpatient_clinic_name { get; set; }//from outpatient_clinic
        public int? outpatient_clinic_ID { get; set; }//from outpatient_clinic
        public string vet_name { get; set; }//from vet_informations

        public string outpatient_clinic_timeslot_name { get; set; }//from outpatient_clinic_timeslot

        public TimeOnly? startat { get; set; }//from outpatient_clinic_timeslot

        public TimeOnly? endat { get; set; }//from outpatient_clinic_timeslot

        public bool? daily_outpatient_clinic_schedule_status { get; set; }//from daily_outpatient_clinic_schedule
    }

    public class create_daily_outpatient_clinic_schedule_DTO
    {
        public DateOnly date { get; set; }
        public int outpatient_clinic_ID { get; set; }
        public bool daily_outpatient_clinic_schedule_status { get; set; }
    }  

    

}
