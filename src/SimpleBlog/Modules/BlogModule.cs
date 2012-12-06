namespace SimpleBlog.Modules
{
    using System;
    using Nancy;
    using SimpleBlog.Service;

    public class BlogModule : NancyModule
    {
        public BlogModule(ISimpleBlogService blogService)
        {
            Before += ctx => {
                ViewBag.currentTime = blogService.CurrentTime;
                ViewBag.isAdmin = Context.CurrentUser != null;
                return null;
            };

            Get["/"] = x => {
                int pageIndex;
                int.TryParse(Request.Query.page, out pageIndex);

                if (pageIndex <= 0) pageIndex = 1;

                var blog = blogService.GetBlog();
                ViewBag.blog = blog;

                var articles = blogService.GetArticles(pageIndex, Convert.ToInt32(blog.pageSize), includeHidden: (bool)ViewBag.isAdmin);
                ViewBag.articles = articles.Item1;
                ViewBag.totalArticles = articles.Item2;
                ViewBag.singleArticle = false;

                return View["index"];
            };

            Get["/{slug}"] = x => {
                var blog = blogService.GetBlog();
                ViewBag.blog = blog;

                var article = blogService.GetArticleBySlug(x.slug);
                if (article == null) return 404;
                ViewBag.article = article;

                ViewBag.singleArticle = true;

                return Negotiate
                    .WithModel((object)article)
                    .WithView("article");
            };

            Get["/{slug}/{file}"] = x => "static file";

            Get["/category/{category}"] = x => "category";

            Get["/rss"] = x => "rss";
        }
    }
}