namespace SimpleBlog.Service
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using MarkdownDeep;
    using Nancy;
    using SimpleBlog.Models;

    public class SimpleBlogFileSystemService : ISimpleBlogService
    {
        private readonly string path;
        private readonly Regex markdownHeaderRegex = new Regex(@"^(\w+):\s*(.*)\s*\n", RegexOptions.Compiled | RegexOptions.Multiline);
        private readonly Markdown markdown = new Markdown();

        public SimpleBlogFileSystemService(IRootPathProvider rootPathProvider)
            : this(System.IO.Path.Combine(rootPathProvider.GetRootPath(), "App_Data"))
        {
        }

        public SimpleBlogFileSystemService(string path)
        {
            this.path = path;
        }

        public string Path
        {
            get { return this.path; }
        }

        public BlogModel GetBlog()
        {
            var blog = new BlogModel();
            blog.Title = "SimpleBlog";
            ;
            return blog;
        }

        public IDictionary<string, string> PreProcessMetadata(string contents, out string body)
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

        public virtual string TransformContent(string input)
        {
            return this.markdown.Transform(input);
        }

        public static string GetStringValue(IDictionary<string, string> dictionary, string key)
        {
            string value;
            return dictionary.TryGetValue(key, out value) ? value : null;
        }
    }
}