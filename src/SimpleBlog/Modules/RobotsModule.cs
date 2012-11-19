namespace SimpleBlog.Modules
{
    using Nancy;

    public class RobotsModule : NancyModule
    {
        public RobotsModule()
        {
            Get["/robots.txt"] = 
                _ => "robots.txt";
        }
    }
}