[assembly: System.Web.PreApplicationStartMethod(
    typeof(SimpleBlogDemo.App_Start.SimpleBlogDemoAppStart), "Initialize")]

namespace SimpleBlogDemo.App_Start
{
    using System.Web.Hosting;
    using System.Web.Routing;
    using SimpleBlog;

    public class SimpleBlogDemoAppStart
    {
        public static void Initialize()
        {
            var service = new FileSystemBlogService(HostingEnvironment.MapPath("~/App_Data"));
            var blogApp = SimpleBlog.App(service);

            RouteTable.Routes.Add(new Route("{*pathInfo}", new SimpleOwinAspNetRouteHandler(blogApp)));
        }
    }
}