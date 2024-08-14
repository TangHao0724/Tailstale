using System.ComponentModel.DataAnnotations;

namespace Tailstale.Hospital_ViewModel
{
    public class Appointments_ViewModel
    {
        [Display(Name = "預約編號")]
        public int AppointmentID { get; set; } //from Appointment

        [Display(Name = "預約日期")]
        public DateOnly Date { get; set; } //from daily_outpatient_clinic_schedule

        [Display(Name = "預約狀態")]
        public string AppointmentStatus { get; set; } //from order status

        [Display(Name = "門診名稱")]
        public string OutpatientClinicName { get; set; } //from outpatient_clinic  

        [Display(Name = "門診時段")]
        public string OutpatientClinicTimeslotName { get; set; } //from outpatient_clinic_timeslot

        [Display(Name = ("醫師"))]
        public string VetName { get; set; } //from vet_information

        [Display(Name = "飼主")]
        public string KeeperName { get; set; } //from keeper

        [Display(Name = "寵物")]
        public string PetName { get; set; } //form pet

        [Display(Name = "寵物ID")]
        public int PetID { get; set; } //用來連到病歷
    }

    public class Edit_Appointments_ViewModel
    {
        [Display(Name = "預約狀態")]
        public string AppointmentStatus { get; set; } //from order status
    }

}
