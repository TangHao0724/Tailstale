namespace Tailstale.Hotel_DTO
{
    public class BookingDetailAndCheckin
    {
        public BookingDetailDTO bookingDetail {  get; set; }
        public List<CheckInDTO> checkInDTOs { get; set; }
    }
}
