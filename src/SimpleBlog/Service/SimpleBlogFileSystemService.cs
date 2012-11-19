namespace SimpleBlog.Service
{
    using Nancy;
    using SimpleBlog.Models;

    public class SimpleBlogFileSystemService : ISimpleBlogService
    {
        private readonly string path;

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
            return new BlogModel();
        }

    }
}