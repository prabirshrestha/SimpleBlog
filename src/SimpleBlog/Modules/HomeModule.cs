namespace SimpleBlog.Modules
{
    using Nancy;
    using SimpleBlog.Service;

    public class HomeModule : NancyModule
    {
        public HomeModule(ISimpleBlogService blogService)
        {
            Get["/"] =
                _ =>
                {
                    var blog = blogService.GetBlog();
                    if (blog == null)
                    {
                        return 404;
                    }

                    dynamic model = new DynamicDictionary();
                    model.Blog = blog;

                    return View["index", model];
                };
        }
    }
}