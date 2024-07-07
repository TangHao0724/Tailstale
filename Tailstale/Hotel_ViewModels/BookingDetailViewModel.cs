using Tailstale.Hotel_DTO;

namespace Tailstale.Hotel_ViewModels
{
    public class BookingDetailViewModel
    {
        public int? bookingID { get; set; }
        public string? roomName { get; set; }

        public int? bdAmount { get; set; }

        public int? bdTotal { get; set; }
        public int bdID { get; set; }
        public RoomDTO room { get; set; }
    }
}
