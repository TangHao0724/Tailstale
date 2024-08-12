namespace Tailstale.Salon_DTO
{
    public class BusinessHourDTO
    {
        public int id { get; set; }
        public int business_ID { get; set; }
        public string business_day { get; set; } // 日期字符串
        public string? open_time { get; set; }   // 開始時間字符串
        public string? close_time { get; set; }  // 結束時間字符串
        public int people_limit { get; set; }
    }
}
