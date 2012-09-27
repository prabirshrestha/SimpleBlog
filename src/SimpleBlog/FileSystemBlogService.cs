
namespace SimpleBlog
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using MarkdownDeep;

    public class FileSystemBlogService : ISimpleBlogService
    {
        private readonly string path;
        private readonly Regex markdownHeaderRegex = new Regex(@"^(\w+):\s*(.*)\s*\n", RegexOptions.Compiled | RegexOptions.Multiline);
        private readonly Markdown markdown = new Markdown();

        public FileSystemBlogService(string path)
        {
            this.path = path;
        }

        public string GetIndexTemplate()
        {
            return File.ReadAllText(Path.Combine(path, @"skin/index.cshtml"));
        }

        public Blog GetBlog()
        {
            var blog = new Blog();

            blog.Title = "dummy blog";
            blog.Description = "dummy description";
            blog.ArticlesCountPerPage = 5;

            return blog;
        }

        public IList<Article> GetPosts(int pageIndex, int pageSize, out int totalPosts)
        {
            var articlePaths = Directory.GetFiles(Path.Combine(this.path, "articles"), "*.markdown")
                .Take(pageIndex * pageSize)
                .ToArray();

            var articles = new List<Article>();

            // todo: order articles
            foreach (var articlePath in articlePaths)
            {
                var article = new Article();
                article.Slug = Path.GetFileNameWithoutExtension(articlePath);

                string body;
                article.Headers = PreProcessHeaders(File.ReadAllText(articlePath), out body);
                article.Content = body;

                article.Title = GetStringValue(article.Headers, "Title") ?? article.Slug;

                articles.Add(article);
            }

            totalPosts = articlePaths.Length;
            return articles;
        }

        public virtual string TransformContent(string input)
        {
            return this.markdown.Transform(input);
        }

        private IDictionary<string, string> PreProcessHeaders(string contents, out string body)
        {
            var match = this.markdownHeaderRegex.Match(contents);

            var matched = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            int length = 0;
            while (match.Success)
            {
                matched[match.Groups[1].Value] = match.Groups[2].Value;
                length += match.Length;
                match = match.NextMatch();
            }

            body = contents.Substring(length);
            body = this.TransformContent(body);
            return matched;
        }

        public static string GetStringValue(IDictionary<string, string> dictionary, string key)
        {
            string value;
            return dictionary.TryGetValue(key, out value) ? value : null;
        }
    }
}
