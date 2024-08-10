using System.ComponentModel.DataAnnotations;

namespace Tailstale.MedRecordDTO
{
    public class MedicalRecordDTO
    {
        [Display(Name = "編號")]
        public int id { get; set; }

        [Required(ErrorMessage = "必填")]
        [Display(Name = "飼主")]
        public int keeper_id { get; set; }

        [Display(Name = "飼主電話")]
        public string keeper_number { get; set; }

        [Display(Name = "飼主姓名")]
        public string keeper_name { get; set; }

        [Required(ErrorMessage = "必填")]
        [Display(Name = "寵物")]
        public int pet_id { get; set; }

        [Display(Name = "寵物名字")]
        public string pet_name { get; set; }

        [Display(Name = "品種")]
        public string pet_breed { get; set; }

        [Display(Name = "年紀")]
        public int? pet_age { get; set; }

        [Required(ErrorMessage = "必填")]
        [Display(Name = "就診時間")]
        public DateTime Datetime { get; set; }


        [Required(ErrorMessage = "必填")]
        [Display(Name = "診別")]
        public int outpatient_clinic_id { get; set; }

        [Display(Name = "體重")]
        public decimal? weight { get; set; }

        [Required(ErrorMessage = "必填")]
        [Display(Name = "主訴")]
        public string complain { get; set; }

        [Required(ErrorMessage = "必填")]
        [Display(Name = "診斷")]
        public string diagnosis { get; set; }

        [Required(ErrorMessage = "必填")]
        [Display(Name = "處置")]
        public string treatment { get; set; }

        [Display(Name = "備註")]
        public string? memo { get; set; }

        [Display(Name = "費用")]
        public int? fee { get; set; }
    }
}
