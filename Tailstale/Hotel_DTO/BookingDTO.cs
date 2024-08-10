using System.ComponentModel.DataAnnotations;
using Tailstale.Hotel_DTO;

namespace Tailstale.Hotel_DTO
{
    public class BookingDTO
    {
        public int BookingID { get; set; }
        public string KeeperName { get; set; }
        public string? HotelName { get; set; }
        public DateTime CheckinDate { get; set; }
        public DateTime CheckoutDate { get; set; }
        public string BookingStatus { get; set; }
        public int BookingTotal { get; set; }
        public DateTime BookingDate { get; set; }
        public List<BookingDetailDTO>? BookingDetailDTOs { get; set; }

    }
    public class BookingDTO1
    {
        public int BookingID { get; set; }
        public string KeeperName { get; set; }
        public string? HotelName { get; set; }
        public string CheckinDate { get; set; }
        public string CheckoutDate { get; set; }
        public string BookingStatus { get; set; }
        public int BookingTotal { get; set; }
        public string BookingDate { get; set; }
        public int? datecount {  get; set; }
        public List<BookingDetailDTO>? BookingDetailDTOs { get; set; }

    }
}