namespace SimpleBlog
{
    using System.Collections.Generic;
    using System.Dynamic;
    using Extensions;
    using Stream;
    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

    class Renderers
    {
        private readonly ISimpleBlogService service;
        private readonly RazorEngine.Templating.ITemplateService template;

        public Renderers(ISimpleBlogService service)
        {
            this.service = service;
            this.template = new RazorEngine.Templating.TemplateService();
        }

        public AppFunc Index(AppFunc next)
        {
            const string templateName = "index";
            this.template.Compile(service.GetIndexTemplate(), typeof(DynamicObject), templateName);

            return
                async env =>
                {
                    env.GetRequestHeaders()
                        .SetHeader("content-type", "text/html");

                    var blog = this.service.GetBlog();

                    int totalPosts;
                    var posts = this.service.GetPosts(1, blog.ArticlesCountPerPage, out totalPosts);

                    var model = new
                                    {
                                        Blog = blog,
                                        Url = new
                                                  {
                                                      RequestPathBase = env.GetRequestPathBase(),
                                                      RequestPath = env.GetRequestPath()
                                                  }.ToDynamicObject(),
                                        Posts = posts
                                    };

                    string html = this.template.Run(templateName, model.ToDynamicObject());

                    await env.GetResponseBody()
                        .WriteStringAsync(html);
                };
        }

        public AppFunc Feed(AppFunc next)
        {
            return
                   async env =>
                   {
                       // http://rssjs.org/
                       var routeParameters = env.GetSimpleOwinRouteParameters();
                       var type = routeParameters["type"];

                       await env.GetResponseBody()
                           .WriteStringAsync("feed " + type);
                   };
        }

        public AppFunc Robots(AppFunc arg)
        {
            return
                async env =>
                {
                    await env.GetResponseBody()
                        .WriteStringAsync("robots.txt");
                };
        }

        public AppFunc Article(AppFunc next)
        {
            return
                async env =>
                {
                    await env.GetResponseBody()
                        .WriteStringAsync("article");
                };
        }

        public AppFunc StaticFile(AppFunc next)
        {
            return
                async env =>
                {
                    await env.GetResponseBody()
                        .WriteStringAsync("static file");
                };
        }

        public AppFunc Category(AppFunc next)
        {
            return
                async env =>
                {
                    await env.GetResponseBody()
                        .WriteStringAsync("category");
                };
        }

        public AppFunc NotFound(AppFunc next)
        {
            return
                async env =>
                {
                    env.SetResponseStatusCode(404);

                    await env.GetResponseBody()
                        .WriteStringAsync("not found");
                };
        }
    }
}