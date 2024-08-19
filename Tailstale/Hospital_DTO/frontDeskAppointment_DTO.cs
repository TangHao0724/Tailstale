namespace Tailstale.Hospital_DTO
{
    public class appointmentRequest_DTO
    { 
        
        public int dailyOutpatientClinicScheduleID { get; set; }
    }

    public class AppointmentComfrim_DTO
    {
        public int dailyOutpatientClinicScheduleID { get; set; }//from daily_outpatient_clinic_schedule        

        public DateOnly date { get; set; }//from daily_outpatient_clinic_schedule
        public string outpatientClinicName { get; set; }//from outpatient_clinic
        public string clinicName { get; set; }//from business
        public string vetName { get; set; }//from vet_informations
        public string outpatientClinicTimeslotName { get; set; }//from outpatient_clinic_timeslot

        public TimeOnly startAt { get; set; }//from outpatient_clinic_timeslot

        public TimeOnly endAt { get; set; }//from outpatient_clinic_timeslot
        public int keeperID { get; set; }
        public string keeperName { get; set; }

        public List<PetInfo_DTO> petInfos {get;set;}
    }

    public class KeeperInfo_DTO
    {
        public int keeperID { get; set; }
        public string keeperName { get; set; }
    }

    public class PetInfo_DTO
    {
        public int petID { get; set; }
        public string petName { get; set; }
    }

    public class NewAppointment_DTO
    {
        public int keeperID { get; set; }
        public int petID { get; set; }
        public int dailyOutpatientClinicScheduleID { get; set; }

    }
}
