using System.ComponentModel.DataAnnotations;

namespace Tailstale.MedRecordDTO
{
    public class HospDTO
    {
        //keeper
        [Required(ErrorMessage = "必填")]
        [Display(Name = "飼主")]
        public int keeper_id { get; set; }

        [Display(Name = "飼主電話")]
        public string keeper_number { get; set; }

        [Display(Name = "飼主姓名")]
        public string keeper_name { get; set; }



        //pet
        [Display(Name = "寵物")]
        public int? pet_id { get; set; }

        [Display(Name = "pet type ID")]
        public int? pet_type_ID { get; set; }

        [Display(Name = "寵物物種")]
        public string species { get; set; }

        [Display(Name = "寵物品種")]
        public string pet_breed { get; set; }

        [Display(Name = "寵物名字")]
        public string pet_name { get; set; }

        [Display(Name = "結紮")]
        public bool? neutered { get; set; }

        [Display(Name = "過敏")]
        public string allergy { get; set; }

        [Display(Name = "年紀")]
        public int? pet_age { get; set; }


        //medical records
        [Display(Name = "看診記錄")]
        public int? medical_records_id { get; set; }

        [Display(Name = "看診時間")]
        public DateTime? Datetime { get; set; }


        //hosp reccords
        [Display(Name = "編號")]
        public int id { get; set; }

        [Display(Name = "入院時間")]
        public DateTime admission_date { get; set; }

        //[Display(Name = "入院時間")]
        //public string? admission_date_view { get; set; }

        [Display(Name = "出院時間")]
        public DateTime? discharge_date { get; set; }

        [Display(Name = "病房")]
        public int? ward_id { get; set; }

        [Display(Name = "備註")]
        public string? memo { get; set; }
    }
}
