namespace SimpleBlog.Service
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using MarkdownDeep;
    using Nancy;
    using SimpleBlog.Models;

    public class SimpleBlogFileSystemService : ISimpleBlogService
    {
        private readonly Regex markdownHeaderRegex = new Regex(@"^(\w+):\s*(.*)\s*\n", RegexOptions.Compiled | RegexOptions.Multiline);
        private readonly Markdown markdown = new Markdown();

        public SimpleBlogFileSystemService(IRootPathProvider rootPathProvider)
            : this(Path.Combine(rootPathProvider.GetRootPath(), "App_Data"))
        {
        }

        public SimpleBlogFileSystemService(string dataPath)
        {
            this.DataPath = dataPath;
        }

        public string DataPath { get; private set; }

        public DateTime CurrentTime { get { return DateTime.UtcNow; } }

        public Blog GetBlog()
        {
            string body;
            var blog = new Blog {
                Metadata = this.PreProcessMetadata(ReadFile(CombinePath(DataPath, "blog")), out body),
            };
            return blog;
        }

        public Tuple<long, IEnumerable<Article>> GetArticles(int pageIndex, int pageSize, bool includeHidden)
        {
            if (pageIndex <= 0)
            {
                pageIndex = 1;
            }

            if (pageSize <= 0)
            {
                pageSize = 5;
            }

            var currentTime = CurrentTime;

            var articles = GetAllArticles();
            if (!includeHidden)
            {
                articles = articles.Where(article => article.IsHidden(currentTime));
            }

            var total = articles.LongCount();

            articles = articles.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            return new Tuple<long, IEnumerable<Article>>(total, articles);
        }

        public Article GetArticleBySlug(string slug, bool includeHidden)
        {
            var article = GetAllArticles().SingleOrDefault(a => a.Slug == slug);
            if (article == null)
                return null;

            if (includeHidden)
            {
                if (article.IsHidden(CurrentTime))
                    return null;
            }

            return article;
        }

        public string GenerateSlug(string input)
        {
            string str = RemoveAccent(input).ToLower();

            str = Regex.Replace(str, @"[^a-z0-9\s-]", ""); // invalid chars           
            str = Regex.Replace(str, @"\s+", " ").Trim(); // convert multiple spaces into one space   
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim(); // cut and trim it   
            str = Regex.Replace(str, @"\s", "-"); // hyphens   

            return str;
        }

        private static string RemoveAccent(string txt)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }

        private IEnumerable<Article> GetAllArticles()
        {
            var articlesFolder = CombinePath(DataPath, "articles");
            var files = Directory.GetFiles(articlesFolder, "*.markdown");

            var articles = new List<Article>();
            foreach (var file in files)
            {
                string content;
                var metadata = PreProcessMetadata(ReadFile(file), out content);

                var article = new Article {
                    Metadata = metadata,
                    RawContent = content,
                    HtmlContent = TransformContent(content)
                };

                if (!metadata.ContainsKey("Slug"))
                {
                    article.Slug = GenerateSlug(metadata.Title);
                }

                articles.Add(article);
            }

            // sort descending
            articles.Sort(new Comparison<dynamic>((x, y) => {
                DateTime xDateTime = x.DateTime;
                DateTime yDateTime = y.DateTime;

                return yDateTime.CompareTo(xDateTime);
            }));

            return articles;
        }

        public virtual dynamic PreProcessMetadata(string contents, out string body)
        {
            var match = this.markdownHeaderRegex.Match(contents);

            var dict = new DynamicDictionary();
            int length = 0;
            while (match.Success)
            {
                dict[match.Groups[1].Value] = match.Groups[2].Value;
                length += match.Length;
                match = match.NextMatch();
            }

            body = contents.Substring(length);
            dict["raw"] = body;
            body = this.TransformContent(body);
            return dict;
        }

        public virtual string TransformContent(string input)
        {
            return this.markdown.Transform(input);
        }

        protected string CombinePath(params string[] paths)
        {
            return Path.Combine(paths);
        }

        protected string ReadFile(string path)
        {
            return File.ReadAllText(path);
        }

        protected string[] GetFiles(string path, string searchPattern)
        {
            return Directory.GetFiles(path, searchPattern);
        }

        protected string GetFileNameWithoutExtension(string file)
        {
            return Path.GetFileNameWithoutExtension(file);
        }
    }
}