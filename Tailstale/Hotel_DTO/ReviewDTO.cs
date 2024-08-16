
namespace Tailstale.Hotel_DTO
{
    public class ReviewDTO
    {
        public int bookingID { get; set; }
        public int? roomID { get; set; }

        // public int? keeper_ID { get; set; }

        public int? reviewRating { get; set; }

        public string reviewText { get; set; }

        // public string reviewDate { get; set; }

    }
    public class ReViewTrans
    {
        public string keeperName { get; set; }
        public int reviewRating { get; set; }
        public string reviewText { get; set; }
        public string reviewDate { get; set; }
    }
}
