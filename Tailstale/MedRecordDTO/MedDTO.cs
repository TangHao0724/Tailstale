using System.ComponentModel.DataAnnotations;

namespace SP.Tailstale.MedRecordDTO
{
    public class InfoDTO
    {
        [Display(Name = "病歷編號")]
        public int id { get; set; }

        [Display(Name = "飼主id")]
        public int? keeper_id { get; set; }

        [Display(Name = "寵物id")]
        public int? pet_id { get; set; }

        [Display(Name = "診別")]
        public int? outpatient_clinic_id { get; set; }

    }

    public class MedDTO
    {
        [Display(Name = "就醫時間")]
        public DateTime? created_at { get; set; }

    }
}
