namespace SimpleBlog.Models
{
    public class BlogModel
    {
        public BlogModel()
        {
            ArticleCountPerPage = 5;
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public int ArticleCountPerPage { get; set; }
    }
}