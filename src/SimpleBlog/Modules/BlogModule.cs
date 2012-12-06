namespace SimpleBlog.Modules
{
    using System;
    using Nancy;
    using Nancy.Responses;
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

                if (pageIndex <= 1)
                {
                    if (Request.Query.page.HasValue) return Response.AsRedirect("~/", RedirectResponse.RedirectType.Permanent);
                    pageIndex = 1;
                }

                var blog = blogService.GetBlog();
                ViewBag.blog = blog;
                ViewBag.pageSize = Convert.ToInt32(blog.pageSize);
                var articles = blogService.GetArticles(pageIndex, ViewBag.pageSize.Value, includeHidden: (bool)ViewBag.isAdmin);
                ViewBag.articles = articles.Item1;
                ViewBag.totalArticles = articles.Item2;
                ViewBag.singleArticle = false;
                ViewBag.pageIndex = pageIndex;
                ViewBag.hasNextPage = pageIndex * ViewBag.pageSize.Value < ViewBag.totalArticles.Value;

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