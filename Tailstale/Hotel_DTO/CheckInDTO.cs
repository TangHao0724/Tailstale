using System.ComponentModel.DataAnnotations;

namespace Tailstale.Hotel_DTO
{
    public class CheckInDTO
    {
       
        public int petID {  get; set; }
        [Display(Name = "寵物名稱")]
        public string petName {  get; set; }
        [Display(Name = "寵物類型")]
        public string petType { get; set; }
        [Display(Name = "寵物生日")]
        public DateOnly? petBirthDay { get; set; }
        [Display(Name = "寵物晶片")]
        public string? petChipID { get; set; }
        [Display(Name = "房間類型")]
        public string? roomName     { get; set; }
       
        public int? roomID { get; set; }
    } 
}
