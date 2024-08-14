namespace Tailstale.Tools
{
    public class BookingID
    {
        public int bookingID { get; set; }
    }
    public class card
    {
        public string cardNumber { get; set; }
    }
    public class BookingIDAndTotal
    {
        public int? bookingID { get; set; }
        public int ToTalAmount { get; set; }
    }
}
