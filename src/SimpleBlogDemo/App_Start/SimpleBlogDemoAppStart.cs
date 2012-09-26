[assembly: System.Web.PreApplicationStartMethod(
    typeof(SimpleBlogDemo.App_Start.SimpleBlogDemoAppStart), "Initialize")]

namespace SimpleBlogDemo.App_Start
{
    using System.Web.Routing;
    using SimpleBlog;

    public class SimpleBlogDemoAppStart
    {
        public static void Initialize()
        {
            var blog = SimpleBlog.App();

            RouteTable.Routes.Add(new Route("{*pathInfo}", new SimpleOwinAspNetRouteHandler(blog)));
        }
    }
}