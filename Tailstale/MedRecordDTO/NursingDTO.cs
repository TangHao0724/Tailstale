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

        [Display(Name = "時間")]
        public DateTime datetime { get; set; }

        [Display(Name = "體重")]
        public decimal? weight { get; set; }

        [Display(Name = "備註")]
        public string memo { get; set; }

        [Display(Name = "生命徵象")]
        public int? VS_id { get; set; }

        [Display(Name = "生物檢驗")]
        public int test_id { get; set; }
    }
}
