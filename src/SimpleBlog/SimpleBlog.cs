namespace SimpleBlog
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Stream;
    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

    public class SimpleBlog
    {
        public static AppFunc App()
        {
            var app = new List<Func<AppFunc, AppFunc>>();

            var router = new RegexRouter(app);

            router.Get(@"^\/()$", Renderers.Index());
            router.Get(@"^\/()feed.xml$", Renderers.Feed());

            return app.ToOwinApp();
        }
    }
}