namespace Tailstale.KImgDTO
{
    public class singleImgDTO
    {
        public int UserID { get; set; }
        public string type_name { get; set; }
        public IFormFile img { get; set; }
        public string img_name { get; set; }
    }
    public class muitiImgDTO
    {
        public int UserID { get; set; }
        public string type_name { get; set; }
        public List<IFormFile> imgs { get; set; }
        public string img_name { get; set; }
    }
    public class GetsingleImgDTO
    {
        public int UserID { get; set; }
        public string type_name { get; set; }
        public string img_name { get; set; }
    }
    public class ImgURLDTO
    {
        public string img_name{ get; set; }
        public string img_url { get; set; }
    }
    public class postImgTypeDTO
    {
        public int ID {  get; set; }
        public string type_name { get; set; }
    }
}
