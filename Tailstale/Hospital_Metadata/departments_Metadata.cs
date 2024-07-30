using System.ComponentModel.DataAnnotations;
using Tailstale.Models;

namespace Tailstale.Hospital_Metadata
{
    public class department_Metadata
    {
        [Display(Name ="科別名稱")]
        [Required(ErrorMessage ="請填寫科別名稱")]
        public string department_name { get; set; }

        [Display(Name ="院所名稱")]        
        public int? business_ID { get; set; }

        [Display(Name = "院所名稱")]
        public virtual business business { get; set; }
    }
}
