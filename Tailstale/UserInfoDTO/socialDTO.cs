namespace Tailstale.UserInfoDTO
{
    public class postArticleDTO
    {
        public int? Keeper_ID { get; set; }
        public int? Business_ID { get; set; }
        public int? parent_ID {  get; set; }
        public string Content { get; set; }
        public bool isPublic {  get; set; }
        public List<IFormFile>? imgs { get; set; }

    }
}
