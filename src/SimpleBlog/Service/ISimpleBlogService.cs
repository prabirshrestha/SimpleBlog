namespace SimpleBlog.Service
{
    using System;
    using System.Collections.Generic;
    using SimpleBlog.Models;

    public interface ISimpleBlogService
    {
        string DataPath { get; }
        DateTime CurrentTime { get; }

        Blog GetBlog();
        Tuple<long, IEnumerable<Article>> GetArticles(int pageIndex, int pageSize, bool includeHidden);
    }
}