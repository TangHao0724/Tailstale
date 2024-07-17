namespace Tailstale.LNR_DTO
{
    public class LNRDTO
    {

    }
    public class LoginDTO
    {
        public string email { get; set; }
        public string password { get; set; }
    }
    public class KRegisterDTO
    {
        public string email { get; set; }
        public string name { get; set; }
        public string password { get; set; }
    }
    public class BRegisterDTO
    {
        public string email { get; set; }
        public string name { get; set; }
        public string password { get; set; }

        public int bType { get; set; }
    }
}