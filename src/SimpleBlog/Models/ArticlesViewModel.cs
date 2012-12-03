namespace SimpleBlog.Models
{
    using System.Collections.Generic;

    public class ArticlesViewModel
    {
        public Blog Blog { get; set; }
        public IEnumerable<Article> Articles { get; set; }
        public long TotalArticles { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }
}