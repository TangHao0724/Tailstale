namespace Tailstale.Tools
{
    public class SendToECPayValue
    {
        //總價格
        public int TotalAmount { get; set; }
        public string? TradeDesc { get; set; }
        //房間名稱
        public string ItemName { get; set; }
        //hotelID
        public string BookingID { get; set; }
    }
}
