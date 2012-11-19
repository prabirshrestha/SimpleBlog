namespace SimpleBlog.Modules
{
    using Nancy;

    public class CategoryModule : NancyModule
    {
        public CategoryModule()
            : base("/category")
        {
            Get["/{category}"] =
                x => "category " + x.category;
        }
    }
}