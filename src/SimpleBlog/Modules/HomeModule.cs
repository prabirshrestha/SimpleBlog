namespace SimpleBlog.Modules
{
    using Nancy;
    using SimpleBlog.Service;

    public class HomeModule : NancyModule
    {
        public HomeModule(ISimpleBlogService blogService)
        {
            Get["/"] =
                _ => "Welcome to SimpleBlog!";
        }
    }
}