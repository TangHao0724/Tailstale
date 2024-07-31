namespace Tailstale.KImgDTO
{
    public class singleImgDTO
    {
        public int UserID { get; set; }
        public string type_name { get; set; }
        public IFormFile img { get; set; }
        public string img_name { get; set; }
    } 
}
