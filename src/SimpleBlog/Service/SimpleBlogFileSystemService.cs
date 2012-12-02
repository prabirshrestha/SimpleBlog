namespace SimpleBlog.Service
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
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

        private dynamic blog;
        private IList<dynamic> articles;

        public SimpleBlogFileSystemService(IRootPathProvider rootPathProvider)
            : this(System.IO.Path.Combine(rootPathProvider.GetRootPath(), "App_Data"))
        {
        }

        public SimpleBlogFileSystemService(string path)
        {
            this.rootPath = path;

            this.ReloadMetadata();
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
                if (!metadata.ContainsKey("title"))
                {
                    throw new ApplicationException("Article does not contain Title - " + file);
                }

                if (!metadata.ContainsKey("date"))
                {
                    throw new ApplicationException("Article does not contain Title - " + file);
                }

                string dt = metadata.date;
                if (dt.Contains("("))
                {
                    dt = dt.Substring(0, dt.LastIndexOf(" ", StringComparison.Ordinal));
                }

                DateTime dateTime;
                if (!DateTime.TryParseExact(dt, "ddd MMM dd yyyy HH:mm:ss 'GMT'zzz", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out dateTime))
                {
                    throw new ApplicationException("Unrecognized date time format " + (string)metadata.date + " in " + GetFileNameWithoutExtnsion(file) + ". Expected format: ddd MMM dd yyyy HH:mm:ss 'GMT'zzz. Sample:  Tue Feb 02 2010 10:16:51 GMT-0600 (CST)");
                }

                metadata["DateTime"] = dateTime;
                metadata["content"] = content;

                if (!metadata.ContainsKey("slug"))
                {
                    metadata.slug = GenerateSlug(metadata.title);
                }

                articles.Add(metadata);
            }

            // sort descending
            articles.Sort(new Comparison<dynamic>((x, y) => {
                DateTime xDateTime = x.DateTime;
                DateTime yDateTime = y.DateTime;

                return yDateTime.CompareTo(xDateTime);
            }));

            return articles;
        }

        public static string GenerateSlug(string phrase)
        {
            string str = RemoveAccent(phrase).ToLower();

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

        public string Path
        {
            get { return this.rootPath; }
        }

        public dynamic GetBlog()
        {
            this.ReloadMetadata();
            return blog;
        }

        public dynamic GetArticles(int pageIndex, int pageSize, out long totalCount)
        {
            if (pageIndex <= 0)
            {
                pageIndex = 1;
            }

            if (pageSize <= 0)
            {
                pageSize = 5;
            }

            totalCount = this.articles.Count;

            return this.articles.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        public dynamic PreProcessMetadata(string contents, out string body)
        {
            var match = this.markdownHeaderRegex.Match(contents);

            var matched = new JsonObject(StringComparer.InvariantCultureIgnoreCase);
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

        public dynamic GetArticleBySlug(string slug)
        {
            return this.articles.SingleOrDefault(a => a.slug == slug);
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

        protected string GetFileNameWithoutExtnsion(string file)
        {
            return System.IO.Path.GetFileNameWithoutExtension(file);
        }

        protected void ReloadMetadata()
        {
            string body;
            this.blog = this.PreProcessMetadata(ReadFile(CombinePath(this.rootPath, "blog")), out body);
            this.articles = this.GetArticles();
        }
    }
}