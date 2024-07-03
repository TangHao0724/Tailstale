namespace Tailstale.Index_DTO
{
    public class UserDTO
    {
        public string name {  get; set; } 
        public string email { get; set; }
        public string password { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
    }
    public class UserDetailDTO
    {
        public int ID { get; set; }

        public string? password { get; set; }

        public string? name { get; set; }

        public string? phone { get; set; }

        public string? email { get; set; }

        public string? address { get; set; }

        public int status { get; set; }

        public DateTime? created_at { get; set; }
    }
    public class UserUpdateDTO
    {
        public int ID { get; set; }
        public string? name { get; set; }
        public string? email { get; set; }
        public string? password { get; set; }
        public string? address { get; set; }
        public string? phone { get; set; }

        public int status { get; set; }









    }
}