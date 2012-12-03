namespace SimpleBlog.Modules
{
    using Nancy;
    using SimpleBlog.Models;
    using SimpleBlog.Service;

    public class BlogModule : NancyModule
    {
        public BlogModule(ISimpleBlogService blogService)
        {
            Before += ctx => {
                ViewBag.IsAdmin = Context.CurrentUser != null;
                ViewBag.Blog = blogService.GetBlog();
                return null;
            };

            Get["/(rss)?"] = x => {
                int pageIndex;
                int.TryParse(Request.Query.page, out pageIndex);

                var viewModel = new ArticlesViewModel {
                    Blog = ViewBag.Blog.Value
                };

                var articlesPaged = blogService.GetArticles(pageIndex, viewModel.Blog.PageSize, includeHidden: (bool)ViewBag.IsAdmin);
                viewModel.TotalArticles = articlesPaged.Item1;
                viewModel.Articles = articlesPaged.Item2;

                ViewBag.SinglePost = false;

                return Negotiate
                    .WithMediaRangeModel("application/javascript", viewModel).WithView("articles/rss/jsonp/index")
                    .WithMediaRangeModel("application/json", viewModel).WithView("articles/rss/json/index")
                    .WithMediaRangeModel("application/xml", viewModel).WithView("articles/rss/xml/index")
                    .WithMediaRangeModel("text/html", viewModel).WithView("articles/articles");
            };

            Get["/{slug}"] = x => {
                Article article = blogService.GetArticleBySlug(x.slug, includeHidden: ViewBag.IsAdmin);

                if (article == null) return 404;

                ViewBag.SinglePost = true;

                return Negotiate
                    .WithMediaRangeModel("text/html", article).WithView("articles/article");
            };
        }
    }
}