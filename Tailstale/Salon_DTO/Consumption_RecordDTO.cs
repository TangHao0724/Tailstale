namespace Tailstale.Salon_DTO
{
    public class Consumption_RecordDTO
    {
        public int id { get; set; }

        public int keeper_id { get; set; }

        public string keeper_name { get; set; }

        public string pet_name { get; set; }

        public int business_ID { get; set; }

        public string? business_name { get; set; }

        public string time { get; set; }

        public int beautician_id { get; set; }

        public string beautician_name { get; set; }

        public string service_name { get; set; }

        public decimal pet_weight { get; set; }

        public int price { get; set; }

        public string end_time { get; set; }

        public string before_photo { get; set; }

        public string after_photo { get; set; }
    }
}
