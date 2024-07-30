namespace Tailstale.Hospital_DTO
{
    public class daily_outpatient_clinic_schedule_DTO
    {
        public DateOnly? date { get; set; }
        public string outpatient_clinic_name { get; set; }
        public int? outpatient_clinic_ID { get; set; }
        public string vet_name { get; set; }
        
        public string outpatient_clinic_timeslot_name { get; set; }

        public TimeOnly? startat { get; set; }

        public TimeOnly? endat { get; set; }
       
        public bool? daily_outpatient_clinic_schedule_status { get; set; }
    }

    public class create_daily_outpatient_clinic_schedule_DTO
    {
        public DateOnly date { get; set; }
        public int? outpatient_clinic_ID { get; set; }
    }
}
