namespace SimpleBlog
{
    using System;
    using Stream;
    using Extensions;
    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

    class Renderers
    {
        public static Func<AppFunc, AppFunc> Index()
        {
            return
                next =>
                async env =>
                {
                    await env.GetResponseBody()
                        .WriteStringAsync("index");
                };
        }

        public static Func<AppFunc, AppFunc> Feed()
        {
            return next =>
                   async env =>
                   {
                       // http://rssjs.org/
                       var routeParameters = env.GetSimpleOwinRouteParameters();
                       var type = routeParameters["type"];

                       await env.GetResponseBody()
                           .WriteStringAsync("feed " + type);
                   };
        }

        public static Func<AppFunc, AppFunc> Article()
        {
            return
                next =>
                async env =>
                {
                    await env.GetResponseBody()
                        .WriteStringAsync("article");
                };
        }

        public static Func<AppFunc, AppFunc> StaticFile()
        {
            return
                next =>
                async env =>
                {
                    await env.GetResponseBody()
                        .WriteStringAsync("static file");
                };
        }

        public static Func<AppFunc, AppFunc> Category()
        {
            return
                next =>
                async env =>
                {
                    await env.GetResponseBody()
                        .WriteStringAsync("category");
                };
        }

        public static Func<AppFunc, AppFunc> NotFound()
        {
            return
                next =>
                async env =>
                {
                    env.SetResponseStatusCode(404);

                    await env.GetResponseBody()
                        .WriteStringAsync("not found");
                };
        }
    }
}