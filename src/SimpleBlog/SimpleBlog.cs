namespace SimpleBlog
{
    using System;
    using System.Collections.Generic;
    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

    public class SimpleBlog
    {
        public static AppFunc App(ISimpleBlogService service)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            var app = new List<Func<AppFunc, AppFunc>>();

            var router = new RegexRouter(app);

            var renderers = new Renderers(service);
            router.Get(@"^\/()$", renderers.Index);
            router.Get(@"^\/()feed.(?<type>xml|json|js)$", renderers.Feed);
            router.Get(@"^\/()robots.txt$", renderers.Robots);
            router.Get(@"^\/([a-f0-9]{40})\/([a-z0-9_-]+)$", renderers.Article);
            router.Get(@"^\/([a-f0-9]{40})\/(.+\.[a-z]{2,4})$", renderers.StaticFile);
            router.Get(@"^\/()([a-z0-9_-]+)$", renderers.Article);
            router.Get(@"^\/()(.+\.[a-z]{2,4})$", renderers.StaticFile);
            router.Get(@"^\/()category\/([\%\.a-z0-9_-]+)$", renderers.Category);
            router.All(@"*", renderers.NotFound);

            return app.ToOwinApp();
        }
    }
}