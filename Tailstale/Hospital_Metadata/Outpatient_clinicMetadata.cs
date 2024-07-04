using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Tailstale.Models;

namespace Tailstale.Hospital_Metadata
{
    public class Outpatient_clinicMetadata
    {
        [Display(Name ="門診名稱")]
        public string name { get; set; }

        [Display(Name = "院所名稱")]
        public virtual business business { get; set; }
        [Display(Name = "院所名稱")]
        public int? business_ID { get; set; }

        [Display(Name = "科別名稱")]
        public virtual department department { get; set; }
        [Display(Name = "科別名稱")]
        public int? department_ID { get; set; }

        [Display(Name = "醫師姓名")]
        public virtual vet_information vet { get; set; }
        [Display(Name = "醫師姓名")]
        public int? vet_ID { get; set; }

        [Display(Name = "日期")]
        public DateOnly date { get; set; }

        [Display(Name = "時間")]
        public TimeOnly time { get; set; }

        [Display(Name = "診療人數上限")]
        public int max_patients { get; set; }
    }
}
