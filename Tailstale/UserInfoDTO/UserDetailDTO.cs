namespace Tailstale.UserInfoDTO
{
    internal class UserDetailDTO
    {
        public int ID { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public int? status { get; set; }
        public DateTime? created_at { get; set; }

    }
}