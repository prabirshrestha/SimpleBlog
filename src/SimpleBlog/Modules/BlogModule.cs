namespace SimpleBlog.Modules
{
    using System.Linq;
    using Nancy;
    using SimpleBlog.Models;
    using SimpleBlog.Service;

    public class BlogModule : NancyModule
    {
        public BlogModule(ISimpleBlogService blogService)
        {
            Get["/{rss?}"] = x => {

                int pageIndex;
                int.TryParse(Request.Query.page, out pageIndex);

                bool isAdmin = Context.CurrentUser != null;

                var blog = blogService.GetBlog();
                var articlesPaged = blogService.GetArticles(pageIndex, blog.PageSize, includeHidden: isAdmin);

                var viewModel = new ArticlesViewModel {
                    Blog = blog,
                    TotalArticles = articlesPaged.Item1,
                    Articles = articlesPaged.Item2,
                };

                ViewBag.SinglePost = false;
                ViewBag.IsAdmin = isAdmin;
                ViewBag.Blog = blog;

                return Negotiate
                    .WithMediaRangeModel("application/javascript", viewModel).WithView("articles/rss/jsonp/index")
                    .WithMediaRangeModel("application/json", viewModel).WithView("articles/rss/json/index")
                    .WithMediaRangeModel("application/xml", viewModel).WithView("articles/rss/xml/index")
                    .WithMediaRangeModel("text/html", viewModel).WithView("articles/articles");
            };
        }
    }
}