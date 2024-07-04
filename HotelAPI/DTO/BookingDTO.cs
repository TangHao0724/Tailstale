namespace WebApplication1.DTO
{
    public class BookingDTO
    {
        public int KeeperId { get; set; }
        public int HotelId { get; set; }
        public DateTime CheckinDate { get; set; }
        public DateTime CheckoutDate { get; set; }
       
    }
}