using System.ComponentModel.DataAnnotations;
using Tailstale.Models;

namespace Tailstale.Hotel_DTO
{
    public class RoomDTO
    {
        public int hotelID { get; set; }
        public int? roomID { get; set; }
        [Display(Name ="適合物種")]
        public string roomSpecies { get; set; }
        [Display(Name = "房間價格")]
        public int? roomPrice { get; set; }
        [Display(Name = "折扣價")]
        public int? roomDiscount { get; set; }
        [Display(Name = "房間數量")]
        public int? roomReserve { get; set; }
        [Display(Name = "房間類型")]
        public int? RoomName { get; set; }
        [Display(Name = "房間描述")]
        public string roomDescrep { get; set; }
      
        public roomType roomType { get; set; }
       
        public List<business_img>? roomImg { get; set; }

    }
    public class EditRoomDTO
    {
        public int roomID { get; set; }
        public int hotelID { get; set; }
        [Display(Name = "適合物種")]
        public string roomSpecies { get; set; }
        [Display(Name = "房間價格")]
        public int? roomPrice { get; set; }
        [Display(Name = "折扣價")]
        public int? roomDiscount { get; set; }
        [Display(Name = "房間數量")]
        public int? roomReserve { get; set; }
        [Display(Name = "房間描述")]
        public string? roomDescrep { get; set; }
        public roomType roomType { get; set; }
        public List<business_img> roomImg{ get; set; }

    }
}