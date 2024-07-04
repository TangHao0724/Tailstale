using Tailstale.Models;

namespace Tailstale.Hotel_ViewModels
{
    public class CreateBookingVM
    {
        public Booking booking { get; set; }
        public List<BookingDetail> bd { get; set; }
        public business business { get; set; }
        public keeper keeper { get; set; }
    }
}
