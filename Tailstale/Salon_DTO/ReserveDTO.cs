namespace Tailstale.Salon_DTO
{
    public class ReserveDTO
    {
        public int id { get; set; }

        public int keeper_id { get; set; }

        public string keeper_name { get; set; }

        public string pet_name { get; set; }

        public int business_ID { get; set; }

        public string? business_name { get; set; }

        public string time { get; set; }

        public string service_name { get; set; }

        public string created_at { get; set; }

        public int? status { get; set; }

        public string? status_name { get; set; }
    }
}
