using Tailstale.Models;

namespace Tailstale.Hotel_DTO
{
    public class FindRoomResultDTO
    {
        public int roomID { get; set; }
        public int roomPrice { get; set; }
        public string roomDescription { get; set; }
        public int roomReserve { get; set; }
        public roomType roomType { get; set; }
        public int hotelID { get; set; }
        public string roomSpecies { get; set; }
        public business business { get; set; }
        public int? priceTotal { get; set; }
        public virtual roomType FK_roomType { get; set; }


    }
}
