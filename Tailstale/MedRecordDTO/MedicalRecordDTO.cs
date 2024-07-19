using System.ComponentModel.DataAnnotations;
using Tailstale.Models;

namespace Tailstale.MedRecordDTO
{
    public class MedicalRecordDTO
    {
        [Display(Name = "病歷編號")]
        public int id { get; set; }

        [Display(Name = "飼主id")]
        public int? keeper_id { get; set; }

        [Display(Name = "寵物id")]
        public int? pet_id { get; set; }

        [Display(Name = "就醫時間")]
        public DateTime? created_at { get; set; }

        [Display(Name = "診別")]
        public int? outpatient_clinic_id { get; set; }

        [Display(Name = "體重")]
        public decimal? weight { get; set; }

        [Display(Name = "入院經過")]
        public string? admission_process { get; set; }

        [Display(Name = "診斷")]
        public string? diagnosis { get; set; }

        [Display(Name = "處置")]
        public string? treatment { get; set; }

        [Display(Name = "生物檢驗")]
        public int? biological_test_id { get; set; }

        [Display(Name = "備註")]
        public string? memo { get; set; }

        [Display(Name = "費用")]
        public int? fee { get; set; }
    }
}
