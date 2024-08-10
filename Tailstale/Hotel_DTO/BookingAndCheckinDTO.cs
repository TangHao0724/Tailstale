using Tailstale.Models;

namespace Tailstale.Hotel_DTO
{
    public class BookingAndCheckinDTO
    {
        public Booking booking { get; set; }
        public List<BookingDetailDTO> bookingDetails { get; set; }
        public List<CheckInDTO> checkinDetails { get; set; }

    }
}
