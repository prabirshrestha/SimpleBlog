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

    public class SimpleBlogFileSystemService : ISimpleBlogService
    {
        private readonly Markdown markdown = new Markdown();

        private static readonly string[] DateFormats = new[] {
            "ddd MMM dd yyyy HH:mm:ss 'GMT'zzz"
        };

        public SimpleBlogFileSystemService(IRootPathProvider rootPathProvider)
            : this(Path.Combine(rootPathProvider.GetRootPath(), "App_Data"))
        {
        }

        public SimpleBlogFileSystemService(string dataPath)
        {
            this.DataPath = dataPath;
        }

        public string DataPath { get; private set; }

        public virtual DateTime CurrentTime { get { return DateTime.UtcNow; } }

        public dynamic GetBlog()
        {
            var metadata = this.PreProcessMetadata(ReadFile(CombinePath(DataPath, "config")));
            if (metadata.ContainsKey("raw")) metadata.Remove("raw");
            if (!metadata.ContainsKey("pageSize")) metadata.pageSize = "5";
            return metadata;
        }

        public Tuple<dynamic, long> GetArticles(int pageIndex, int pageSize, bool includeHidden)
        {
            var currentTime = CurrentTime;
            var articles = GetAllArticles().Where(a => a.date < currentTime);

            if (!includeHidden)
            {
                articles = articles.Where(a => a.draft == false);
            }

            foreach (var article in articles)
            {
                article.html = TransformContent(article.raw);
            }

            return new Tuple<dynamic, long>(articles.Skip((pageIndex - 1) * pageSize).Take(pageSize), articles.LongCount());
        }

        private IEnumerable<dynamic> GetAllArticles()
        {
            var articlesFolder = CombinePath(DataPath, "articles");
            var files = Directory.GetFiles(articlesFolder, "*.markdown");

            var articles = new JsonArray();

            foreach (var file in files)
            {
                dynamic metadata = PreProcessMetadata(ReadFile(file));

                if (!metadata.ContainsKey("title")) throw new ApplicationException("title required for " + GetFileNameWithoutExtension(file));
                if (!metadata.ContainsKey("date")) throw new ApplicationException("date required for " + GetFileNameWithoutExtension(file));
                metadata.date = ParseAsDateTime(metadata.date);

                if (!metadata.ContainsKey("slug")) metadata.slug = GenerateSlug(metadata.title);
                metadata.draft = metadata.ContainsKey("draft") ? bool.Parse(metadata.draft) : false;
                metadata.comments = metadata.ContainsKey("comments") ? bool.Parse(metadata.comments) : true;

                articles.Add(metadata);
            }

            return articles;
        }

        private DateTime ParseAsDateTime(dynamic date)
        {
            string dt = date ?? string.Empty;
            if (dt.Contains("("))
            {
                dt = dt.Substring(0, dt.LastIndexOf(" ", StringComparison.Ordinal));
            }

            DateTime dateTime;
            if (!DateTime.TryParseExact(dt, DateFormats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out dateTime))
            {
                return DateTime.MaxValue;
            }

            return dateTime;
        }

        public string GenerateSlug(string str)
        {
            str = RemoveAccent(str).ToLower();

            str = Regex.Replace(str, @"[^a-z0-9\s-]", ""); // invalid chars           
            str = Regex.Replace(str, @"\s+", " ").Trim(); // convert multiple spaces into one space   
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim(); // cut and trim it   
            str = Regex.Replace(str, @"\s", "-"); // hyphens   

            return str;
        }

        public virtual string TransformContent(string content)
        {
            return this.markdown.Transform(content);
        }

        public dynamic GetArticleBySlug(string slug)
        {
            var article = GetAllArticles().SingleOrDefault(a => a.slug == slug);
            if (article == null) return null;
            article.html = TransformContent(article.raw);
            return article;
        }

        private static string RemoveAccent(string txt)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }

        public virtual dynamic PreProcessMetadata(string contents)
        {
            using (var reader = new StringReader(contents))
            {
                var dict = new JsonObject();
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        dict["raw"] = reader.ReadToEnd();
                        break;
                    }
                    else
                    {
                        var split = line.Split(new[] { ':' }, 2);
                        if (split.Length == 2)
                            dict[split[0].Trim()] = split[1].Trim();
                    }
                }

                if (!dict.ContainsKey("raw")) dict["raw"] = string.Empty;

                return dict;
            }
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