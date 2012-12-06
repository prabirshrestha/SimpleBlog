namespace SimpleBlog.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
                if (pageIndex == 1)
                {
                    ViewBag.hasPreviousPage = false;
                }
                else if (((IEnumerable<dynamic>)ViewBag.articles.Value).Any())
                {
                    ViewBag.hasPreviousPage = true;
                }
                else
                {
                    var redirectPageIndex = (int)Math.Round((double)ViewBag.totalArticles.Value / ViewBag.pageSize.Value);
                    return redirectPageIndex <= 1 ? Response.AsRedirect("~/") : Response.AsRedirect("~/?page=" + redirectPageIndex);
                }

                ViewBag.singleArticle = false;
                ViewBag.pageIndex = pageIndex;
                ViewBag.hasNextPage = pageIndex * ViewBag.pageSize.Value < ViewBag.totalArticles.Value;

                return View["index"];
            };

            Get["/{slug}"] = x => {
                var blog = blogService.GetBlog();
                ViewBag.blog = blog;

                var article = blogService.GetArticleBySlug(x.slug.Value, includeHidden: ViewBag.isAdmin.Value);
                if (article == null) return 404;
                ViewBag.article = article;

                ViewBag.singleArticle = true;

                return Negotiate
                    .WithModel((object)article)
                    .WithView("article");
            };

            Get["/{slug}/edit"] = x => {
                return "edit";
            };

            Get["/{slug}/{file}"] = x => {
                var attachment = blogService.GetAttachmentForSlug(x.slug, x.file, includeHidden: ViewBag.isAdmin.Value);
                if (attachment == null) return 404;
                return new StreamResponse(() => attachment, (string)MimeTypes.GetMimeType(x.file));
            };

            Get["/category/{category}"] = x => "category";

            Get["/rss"] = x => "rss";
        }
    }
}