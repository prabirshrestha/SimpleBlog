﻿namespace SimpleBlog.Modules
{
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

                var blog = blogService.GetBlog();
                var articlesPaged = blogService.GetArticles(pageIndex, blog.PageSize, includeHidden: Context.CurrentUser != null);

                var viewModel = new ArticlesViewModel {
                    Blog = blog,
                    TotalArticles = articlesPaged.Item1,
                    Articles = articlesPaged.Item2,
                };

                return Negotiate
                    .WithMediaRangeModel("application/javascript", viewModel).WithView("articles/rss/jsonp/index")
                    .WithMediaRangeModel("application/json", viewModel).WithView("articles/rss/json/index")
                    .WithMediaRangeModel("application/xml", viewModel).WithView("articles/rss/xml/index")
                    .WithMediaRangeModel("text/html", viewModel).WithView("articles/articles");
            };
        }
    }
}