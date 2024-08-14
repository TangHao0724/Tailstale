using System.ComponentModel.DataAnnotations;

namespace Tailstale.MedRecordDTO
{
    public class MedicalRecordDTO
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
        public int pet_id { get; set; }

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



        //opc id有了
        [Display(Name = "診所名稱")]
        public string outpatient_clinic_name { get; set; }

        [Display(Name = "獸醫名字")]
        public int? vet_ID { get; set; }



        //daily opc
        [Display(Name = "每日預約")]
        public int daily_outpatient_clinic_schedule_ID { get; set; }

        [Display(Name = "預約看診日")] //真正看診日
        public DateOnly? date { get; set; }

        //Appointments不需要
        //從FK_daily_opc去抓keeper,pet的id



        //medical_record
        [Display(Name = "編號")]
        public int id { get; set; }

        [Required(ErrorMessage = "必填")]
        [Display(Name = "看診時間")]
        public DateTime Datetime { get; set; }

        [Required(ErrorMessage = "必填")]
        [Display(Name = "診別")]
        public int outpatient_clinic_id { get; set; }

        [Display(Name = "體重")]
        public decimal? weight { get; set; }

        [Required(ErrorMessage = "必填")]
        [Display(Name = "主訴")]
        public string complain { get; set; }

        [Required(ErrorMessage = "必填")]
        [Display(Name = "診斷")]
        public string diagnosis { get; set; }

        [Display(Name = "交代事項")]
        public string? memo { get; set; }

        [Display(Name = "費用")]
        public int? fee { get; set; }



        //vital_sign_record



        //medical_orders



        //medicine_orders

        //medicine




        //biological test order


        //biological test


        //medical imaging order


        //medical imaging

        //image files

        //surgery
    }
}
