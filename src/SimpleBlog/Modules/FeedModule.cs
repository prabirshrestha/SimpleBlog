namespace SimpleBlog.Modules
{
    using Nancy;

    public class FeedModule : NancyModule
    {
        public FeedModule()
        {
            Get["/feed.{format}"] =
                x => "feed " + x.format;
        }
    }
}