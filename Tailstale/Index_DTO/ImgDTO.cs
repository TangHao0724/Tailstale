using Tailstale.Models;

namespace Tailstale.Index_DTO
{
    public class singleImgDTO
    {
        public IFormFile img { get; set; }
        public int User_id { get; set; }
        public string type_name { get; set; }
        public string Imgname { get; set; }

    }
    public class ImgTypeDTO
    {
        public int User_id { get; set; }
        public string type_name { get; set; }

    }
}
