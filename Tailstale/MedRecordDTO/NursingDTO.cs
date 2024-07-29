using System.ComponentModel.DataAnnotations;

namespace Tailstale.MedRecordDTO
{
    public class NursingDTO
    {
        [Display(Name ="記錄編號")]
        public int id { get; set; }

        [Display(Name = "寵物id")]
        public int? pet_id { get; set; }

        [Display(Name = "住院記錄")]
        public int hosp_history_id { get; set; }

        [Display(Name = "時間")]
        public DateTime datetime { get; set; }

        [Display(Name = "體重")]
        public decimal? weight { get; set; }

        [Display(Name = "內容")]
        public string? memo { get; set; }

        [Display(Name = "生命徵象")]
        public int? vs_id { get; set; }
        
        [Display(Name = "生命徵象備註")]
        public string vs_memo { get; set; }
    }
}
