namespace SimpleBlog
{
    using System.Collections.Generic;

    public interface ISimpleBlogService
    {
        string GetIndexTemplate();

        Blog GetBlog();

        IList<Article> GetPosts(int pageIndex, int pageSize, out int totalPosts);
    }
}