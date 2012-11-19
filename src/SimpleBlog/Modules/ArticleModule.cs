namespace SimpleBlog.Modules
{
    using Nancy;

    public class ArticleModule : NancyModule
    {
        public ArticleModule()
        {
            Get["/{article}"] =
                x => "article " + x.article;

            Get["/{article}/{file}"] =
                x => "article file " + x.article + " " + x.file;
        }
    }
}