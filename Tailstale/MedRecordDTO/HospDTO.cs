using System.ComponentModel.DataAnnotations;
using Tailstale.Models;

namespace Tailstale.MedRecordDTO
{
    public class HospDTO
    {
        [Display(Name= "id")]
        public int id { get; set; }

        [Display(Name = "就醫紀錄")]
        public int? medical_record_id { get; set; }

        [Display(Name = "入院時間")]
        public DateTime admission_date { get; set; }

        [Display(Name = "出院時間")]
        public DateTime? discharge_date { get; set; }

        [Display(Name = "護理紀錄")]
        public int? nursing_record_id { get; set; }

        [Display(Name = "病房")]
        public int? ward_id { get; set; }

        [Display(Name = "備註")]
        public string memo { get; set; }
    }
}
