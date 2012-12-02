namespace SimpleBlog.Modules
{
    using Nancy;
    using SimpleBlog.Service;

    public class ArticleModule : NancyModule
    {
        public ArticleModule(ISimpleBlogService blogService)
        {
            Get["/{slug}"] = x => {
                var blog = blogService.GetBlog();
                var article = blogService.GetArticleBySlug(x.slug);

                if (article == null) return 404;

                ViewBag.blog = blog;
                ViewBag.article = article;
                ViewBag.singlePost = true;

                return View["article"];
            };

            Get["/{article}/{file}"] =
                x => "article file " + x.article + " " + x.file;
        }
    }
}