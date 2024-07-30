using System.ComponentModel.DataAnnotations;
using Tailstale.Models;

namespace Tailstale.Hospital_ViewModel
{
    public class outpatient_clinic_ViewModel
    {
        public int outpatient_clinic_ID { get; set; }
        public int? outpatient_clinic_timeslot_ID { get; set; }

        [Display(Name = "診別名稱")]
        [Required(ErrorMessage ="請填寫診別名稱")]
        public string outpatient_clinic_timeslot_name { get; set; }

        [Display(Name = "開始時間")]
        [Required(ErrorMessage ="請設置門診開始時間")]
        public TimeOnly? startat { get; set; }

        [Display(Name = "結束時間")]
        [Required(ErrorMessage = "請設置門診結束時間")]
        public TimeOnly? endat { get; set; }

        [Display(Name = "門診名稱")]
        [Required(ErrorMessage = "請填寫門診名稱")]
        public string outpatient_clinic_name { get; set; }

        [Display(Name = "醫師姓名")]
        [Required(ErrorMessage = "請選擇醫師")]
        public int? vet_ID { get; set; }

        [Display(Name = "醫師姓名")]        
        public string? vet_name { get; set; }

        [Display(Name = "看診日")]
        [Required(ErrorMessage ="請選擇看診日")]
        public string dayofweek { get; set; }

        [Display(Name ="人數限制")]
        [Required(ErrorMessage ="請設置看診人數上限")]
        public int? max_patients { get; set; }

        [Display(Name ="狀態")]
        public bool status { get; set; }
    }

    public class edit_outpatient_clinic_ViewModel
    {
        public int outpatient_clinic_ID { get; set; }
        [Display(Name = "門診名稱")]
        [Required(ErrorMessage = "請填寫門診名稱，例：(犬貓)一般外科門診、中獸醫針灸門診")]
        public string outpatient_clinic_name { get; set; }

        [Display(Name = "人數限制")]
        [Required(ErrorMessage = "請設置看診人數上限")]
        public int? max_patients { get; set; }

        [Display(Name = "狀態")]
        public bool status { get; set; }
    }
}
