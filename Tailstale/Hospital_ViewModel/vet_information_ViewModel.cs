using System.ComponentModel.DataAnnotations;
using Tailstale.Models;

namespace Tailstale.Hospital_ViewModel
{
    public class vet_information_ViewModel
    {
        public int vet_ID { get; set; }

        [Required(ErrorMessage = "醫師姓名為必填欄位")]
        [Display(Name = "醫師姓名")]
        public string vet_name { get; set; }

        [Required(ErrorMessage = "執照號碼為必填欄位")]
        [Display(Name = "執照號碼")]
        public string license_number { get; set; }

        [Required(ErrorMessage = "學經歷簡介為必填欄位")]
        [StringLength(500, ErrorMessage = "請勿填寫超過字數上限500字")]
        [Display(Name = "學經歷簡介")]
        public string profile { get; set; }

        [Display(Name = "院所名稱")]
        public int? business_ID { get; set; }
        [Display(Name = "院所名稱")]
        public virtual business? business { get; set; }
        [Display(Name = "科別名稱")]
        public int? department_ID { get; set; }
        [Display(Name = "科別名稱")]
        public virtual department? department { get; set; }

        public int? business_img_ID { get; set; }
        public virtual business_img? business_img { get; set; }

        [Display(Name = "醫師照片")]
        public string? URL { get; set; }

        [Display(Name = "照片名稱")]
        public string? name { get; set; }
        [Display(Name ="狀態")]        
        public bool employment_status { get; set; }

    }

    public class insert_vet_information_ViewModel
    {
        [Required(ErrorMessage = "醫師姓名為必填欄位")]
        [Display(Name ="醫師姓名")]
        public string vet_name { get; set; }

        [Required(ErrorMessage = "執照號碼為必填欄位")]
        [Display(Name = "執照號碼")]
        public string license_number { get; set; }

        [Required(ErrorMessage = "學經歷簡介為必填欄位")]
        [StringLength(500, ErrorMessage = "請勿填寫超過字數上限500字")]
        [Display(Name = "學經歷簡介")]
        public string profile { get; set; }

        [Display(Name = "院所名稱")]
        public int business_ID { get; set; }
        [Display(Name = "科別名稱")]
        public int department_ID { get; set; }

        [Display(Name = "醫師照片")]
        public string URL { get; set; }   

        [Display(Name = "狀態")]
        public bool employment_status { get; set; }


    }
}
