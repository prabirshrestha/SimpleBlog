namespace SimpleBlog.Modules
{
    using Nancy;

    public class ArticleModule : NancyModule
    {
        public ArticleModule()
        {
            Get["/{article}"] =
                x => "article " + x.article;
        }
    }
}