namespace Tailstale.Salon_DTO
{
    public class BusinessHourDTO
    {
        public int Id { get; set; }
        public int BusinessId { get; set; }
        public string BusinessDay { get; set; } // 日期字符串
        public string? OpenTime { get; set; }   // 開始時間字符串
        public string? CloseTime { get; set; }  // 結束時間字符串
        public int PeopleLimit { get; set; }
    }
}
