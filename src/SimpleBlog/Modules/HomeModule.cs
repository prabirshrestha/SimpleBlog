namespace SimpleBlog.Modules
{
    using System;
    using Nancy;
    using SimpleBlog.Service;

    public class HomeModule : NancyModule
    {
        public HomeModule(ISimpleBlogService blogService)
        {
            Get["/"] = _ => {
                var blog = blogService.GetBlog();

                long totalArticles;

                dynamic model = new JsonObject(StringComparer.InvariantCultureIgnoreCase);
                model.blog = blog;
                model.articles = blogService.GetArticles(1, Convert.ToInt32(blog.articleCountPerPage), out totalArticles);
                model.totalArticlesCount = totalArticles;

                int pageIndex = 0;
                var __ = Request.Query.page.HasValue && int.TryParse(Request.Query.page, out pageIndex);
                model.page = pageIndex;

                return View["index", model];
            };
        }
    }
}