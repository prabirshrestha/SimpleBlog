namespace SimpleBlog
{
    using System;
    using Stream;
    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

    class Renderers
    {
        public static Func<AppFunc, AppFunc> Index()
        {
            return next => async env =>
                                     {
                                         await env.GetResponseBody()
                                             .WriteStringAsync("index");
                                     };
        }

        public static Func<AppFunc, AppFunc> Feed()
        {
            return next => async env =>
                                     {
                                         await env.GetResponseBody()
                                             .WriteStringAsync("feed");
                                     };
        }
    }
}