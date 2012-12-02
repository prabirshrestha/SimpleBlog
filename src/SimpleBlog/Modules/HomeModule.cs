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

                int pageIndex = 0;
                var y = Request.Query.page.HasValue && int.TryParse(Request.Query.page, out pageIndex);

                long totalArticles;
                var articles = blogService.GetArticles(pageIndex, Convert.ToInt32(blog.artcleCountPerPage), out totalArticles);

                ViewBag.blog = blog;
                ViewBag.articles = articles;
                ViewBag.totalArticles = totalArticles;
                ViewBag.singlePost = false;

                return View["index"];
            };
        }
    }
}