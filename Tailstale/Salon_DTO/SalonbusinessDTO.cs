namespace Tailstale.Salon_DTO
{
    public class SalonbusinessDTO
    {
        public int ID { get; set; }

        public int? type_ID { get; set; }

        public string name { get; set; }

        public string email { get; set; }

        public string phone { get; set; }

        public string address { get; set; }

        public string geoJson { get; set; }

        public string license_number { get; set; }

        public int? business_status { get; set; }

        public string description { get; set; }

        public string photo_url { get; set; }

        public string? created_at { get; set; }
    }
}
