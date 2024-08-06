namespace Tailstale.Salon_DTO
{
    public class Sbusiness_imgDTO
    {
        public int ID { get; set; }

        public int? img_type_id { get; set; }

        public string URL { get; set; }

        public string name { get; set; }

        public int? FK_business_id { get; set; }

        public string business_name { get; set; }

        public string typename { get; set; }

        public string? created_at { get; set; }
    }
}
