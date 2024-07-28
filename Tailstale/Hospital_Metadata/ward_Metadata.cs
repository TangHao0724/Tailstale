using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Tailstale.Models;


namespace Tailstale.Hospital_Metadata
{
    internal class ward_Metadata
    {
        [Required]
        [Display(Name ="病房名稱")]
        public string ward_name { get; set; }

        [Display(Name = "院所名稱")]
        public int? business_ID { get; set; }

        [Display(Name = "院所名稱")]
        public virtual business business { get; set; }

        [Required]
        [Display(Name ="狀態")]
        public bool ward_status { get; set; }

        [Display(Name ="備註")]
        public string memo { get; set; }
    }
}
