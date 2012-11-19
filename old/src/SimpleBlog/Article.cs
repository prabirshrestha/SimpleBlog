namespace SimpleBlog
{
    using System.Collections.Generic;

    public class Article
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Content { get; set; }

        public IDictionary<string, string> Metadata { get; set; }
    }
}