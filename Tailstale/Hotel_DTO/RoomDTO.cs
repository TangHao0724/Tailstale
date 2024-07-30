using Tailstale.Models;

namespace Tailstale.Hotel_DTO
{
    public class RoomDTO
    {
        public int hotelID { get; set; }
        public int? roomID { get; set; }
        public string roomSpecies { get; set; }

        public int? roomPrice { get; set; }

        public int? roomDiscount { get; set; }

        public int? roomReserve { get; set; }

        public int? RoomName { get; set; }
        public string roomDescrep { get; set; }
        public roomType roomType { get; set; }
       public List<business_img>? roomImg { get; set; }

    }
    public class EditRoomDTO
    {
        public int roomID { get; set; }
        public int hotelID { get; set; }
        public string roomSpecies { get; set; }

        public int? roomPrice { get; set; }

        public int? roomDiscount { get; set; }

        public int? roomReserve { get; set; }
        public string? roomDescrep { get; set; }
        public roomType roomType { get; set; }
        public List<business_img> roomImg{ get; set; }

    }
}