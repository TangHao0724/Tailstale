using System.ComponentModel.DataAnnotations;
using Tailstale.Models;

namespace Tailstale.MedRecordDTO
{
    public class NursingDTO
    {
        [Display(Name ="記錄編號")]
        public int id { get; set; }

        [Display(Name = "寵物id")]
        public int? pet_id { get; set; }

        [Display(Name = "住院紀錄")]
        public int hosp_history_id { get; set; }

        [Display(Name = "時間")]
        public DateTime datetime { get; set; }

        [Display(Name = "體重")]
        public decimal? weight { get; set; }

        [Display(Name = "治療項目")]
        public string? memo { get; set; }

        [Display(Name = "生命徵象")]
        public int? VS_id { get; set; }
    }
}
