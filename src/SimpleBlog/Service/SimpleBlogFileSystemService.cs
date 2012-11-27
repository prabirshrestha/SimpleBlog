namespace SimpleBlog.Service
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using MarkdownDeep;
    using Nancy;
    using System.Linq;

    public class SimpleBlogFileSystemService : ISimpleBlogService
    {
        private readonly string rootPath;
        private readonly Regex markdownHeaderRegex = new Regex(@"^(\w+):\s*(.*)\s*\n", RegexOptions.Compiled | RegexOptions.Multiline);
        private readonly Markdown markdown = new Markdown();

        private readonly dynamic blog;
        private IList<dynamic> articles;

        public SimpleBlogFileSystemService(IRootPathProvider rootPathProvider)
            : this(System.IO.Path.Combine(rootPathProvider.GetRootPath(), "App_Data"))
        {
        }

        public SimpleBlogFileSystemService(string path)
        {
            this.rootPath = path;

            string body;
            this.blog = this.PreProcessMetadata(ReadFile(CombinePath(this.rootPath, "blog")), out body);

            this.articles = this.GetArticles();
        }

        private dynamic GetArticles()
        {
            var articlesFolder = CombinePath(this.rootPath, "articles");
            var files = Directory.GetFiles(articlesFolder, "*.markdown");

            var articles = new JsonArray();
            foreach (var file in files)
            {
                string content;
                var metadata = PreProcessMetadata(ReadFile(file), out content);
                metadata["content"] = content;
                articles.Add(metadata);
            }

            return articles;
        }

        public string Path
        {
            get { return this.rootPath; }
        }

        public dynamic GetBlog()
        {
            return blog;
        }

        public dynamic GetArticles(int pageIndex, int pageSize, out long totalCount)
        {
            totalCount = this.articles.Count;

            return this.articles.Skip(pageIndex * pageSize).Take(pageIndex).ToList();
        }

        public dynamic PreProcessMetadata(string contents, out string body)
        {
            var match = this.markdownHeaderRegex.Match(contents);

            var matched = new JsonObject();
            int length = 0;
            while (match.Success)
            {
                matched[match.Groups[1].Value] = match.Groups[2].Value;
                length += match.Length;
                match = match.NextMatch();
            }

            body = contents.Substring(length);
            matched["raw"] = body;
            body = this.TransformContent(body);
            return matched;
        }

        public virtual string TransformContent(string input)
        {
            return this.markdown.Transform(input);
        }

        public static string GetStringValue(IDictionary<string, string> dictionary, string key)
        {
            string value;
            return dictionary.TryGetValue(key, out value) ? value : null;
        }

        protected string CombinePath(params string[] paths)
        {
            return System.IO.Path.Combine(paths);
        }

        protected string ReadFile(string path)
        {
            return File.ReadAllText(path);
        }

        protected string[] GetFiles(string path, string searchPattern)
        {
            return Directory.GetFiles(path, searchPattern);
        }
    }
}