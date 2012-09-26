namespace SimpleBlog
{
    using Extensions;
    using Stream;
    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

    class Renderers
    {
        public AppFunc Index(AppFunc next)
        {
            return
                async env =>
                {
                    await env.GetResponseBody()
                        .WriteStringAsync("index");
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