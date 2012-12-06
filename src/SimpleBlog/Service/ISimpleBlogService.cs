namespace SimpleBlog.Service
{
    using System;

    public interface ISimpleBlogService
    {
        string DataPath { get; }
        DateTime CurrentTime { get; }

        dynamic GetBlog();
        Tuple<dynamic, long> GetArticles(int pageIndex, int pageSize, bool includeHidden);

        string GenerateSlug(string str);
        string TransformContent(string content);
        dynamic GetArticleBySlug(string slug);
    }
}