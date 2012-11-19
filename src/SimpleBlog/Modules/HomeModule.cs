namespace SimpleBlog.Modules
{
    using Nancy;

    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/"] = 
                _ => "Welcome to SimpleBlog!";
        }
    }
}