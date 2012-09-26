namespace SimpleBlog
{
    using System;
    using System.Collections.Generic;
    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

    public class SimpleBlog
    {
        public static AppFunc App()
        {
            var app = new List<Func<AppFunc, AppFunc>>();

            var router = new RegexRouter(app);

            router.Get(@"^\/()$", Renderers.Index());
            router.Get(@"^\/()feed.(?<type>xml|json|js)$", Renderers.Feed());
            router.Get(@"^\/([a-f0-9]{40})\/([a-z0-9_-]+)$", Renderers.Article());
            router.Get(@"^\/([a-f0-9]{40})\/(.+\.[a-z]{2,4})$", Renderers.StaticFile());
            router.Get(@"^\/()([a-z0-9_-]+)$", Renderers.Article());
            router.Get(@"^\/()(.+\.[a-z]{2,4})$", Renderers.StaticFile());
            router.Get(@"^\/()category\/([\%\.a-z0-9_-]+)$", Renderers.Category());
            router.All(@"*", Renderers.NotFound());

            return app.ToOwinApp();
        }
    }
}