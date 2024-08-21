using System.ComponentModel.DataAnnotations;

namespace Tailstale.MedRecordDTO
{
    public class VSrecordDTO
    {
        [Display(Name = "生命徵象id")]
        public int vs_id { get; set; }

        [Display(Name = "測量時間")]
        public DateTime? taketime { get; set; }

        [Display(Name = "心跳")]
        public int? HR { get; set; }

        [Display(Name = "收縮壓")]
        public int? SBP { get; set; }

        [Display(Name = "舒張壓")]
        public int? DBP { get; set; }

        [Display(Name = "體溫")]
        public decimal? BT { get; set; }

        [Display(Name = "呼吸次數")]
        public int? RR { get; set; }

        [Display(Name = "血氧")]
        public int? SpO2 { get; set; }

        [Display(Name = "排尿")]
        public int? UO { get; set; }

        [Display(Name = "備註")]
        public string vs_memo { get; set; }

        [Display(Name = "mr_id")]
        public int? medical_records_id { get; set; }
    }
}
