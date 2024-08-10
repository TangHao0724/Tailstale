namespace Tailstale.Hotel_DTO
{
    public class CheckInDTO
    {
        public int petID {  get; set; }
        public string petName {  get; set; }
        public string petType { get; set; }
        public DateOnly? petBirthDay { get; set; }
        public string? petChipID { get; set; }
        public string? roomName     { get; set; }
        public int? roomID { get; set; }
    } 
}
