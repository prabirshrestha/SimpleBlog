namespace SimpleBlog.Modules
{
    using System;
    using Nancy;
    using SimpleBlog.Service;

    public class ArticleModule : NancyModule
    {
        public ArticleModule(ISimpleBlogService blogService)
        {
            Get["/{slug}"] = x => {
                var blog = blogService.GetBlog();

                dynamic model = new JsonObject(StringComparer.InvariantCultureIgnoreCase);
                model.blog = blog;
                model.article = blogService.GetArticleBySlug(x.slug);

                if (model.article == null)
                    return 404;

                return View["article", model];
            };

            Get["/{article}/{file}"] =
                x => "article file " + x.article + " " + x.file;
        }
    }
}