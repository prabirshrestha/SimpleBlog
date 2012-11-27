namespace SimpleBlog.Service
{

    public interface ISimpleBlogService
    {
        dynamic GetBlog();
        dynamic GetArticles(int pageIndex, int pageSize, out long totalCount);

        string TransformContent(string input);
    }
}