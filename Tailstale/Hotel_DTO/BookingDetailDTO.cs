using Tailstale.Models;

namespace Tailstale.Hotel_DTO
{
    public class BookingDetailDTO
    {

        public int? bookingID { get; set; }
        public string? roomName { get; set; }
        //房價
        public int roomPrice {  get; set; }
        //數量
        public int? bdAmount { get; set; }
        //小計
        public int? bdTotal { get; set; }
        
    }
}
