namespace SimpleBlog
{
    using System.IO;
    using Nancy;
    using SimpleBlog.Service;

    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            var rootPath = container.Resolve<IRootPathProvider>().GetRootPath();
            var blogService = new SimpleBlogFileSystemService(Path.Combine(rootPath, "App_Data"));
            container.Register<ISimpleBlogService>(blogService);
        }
    }
}