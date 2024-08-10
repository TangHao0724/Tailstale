using System.ComponentModel.DataAnnotations;

namespace Tailstale.MedRecordDTO
{
    public class HospDTO
    {
        [Display(Name= "編號")]
        public int id { get; set; }

        [Display(Name = "就診記錄")]
        public int? medical_records_id { get; set; }

        [Display(Name = "就診時間")]
        public DateTime? Datetime { get; set; }

        [Display(Name = "入院時間")]
        public DateTime admission_date { get; set; }

        [Display(Name = "入院時間")]
        public string? admission_date_view { get; set; }

        [Display(Name = "出院時間")]
        public DateTime? discharge_date { get; set; }

        [Display(Name = "護理記錄")]
        public int? nursing_record_id { get; set; }
        
        [Display(Name = "護理記錄時間")]
        public DateTime? nursing_record_datetime { get; set; }

        [Display(Name = "病房")]
        public int? ward_id { get; set; }

        [Display(Name = "備註")]
        public string? memo { get; set; }

        [Display(Name = "寵物id")]
        public int? pet_id { get; set; }
    }
}
