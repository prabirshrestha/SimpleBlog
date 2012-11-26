using Cassette;
using Cassette.Scripts;
using Cassette.Stylesheets;

namespace SimpleBlog
{
    using System.IO;
    using System.Linq;
    using Nancy;

    /// <summary>
    /// Configures the Cassette asset bundles for the web application.
    /// </summary>
    public class CassetteBundleConfiguration : IConfiguration<BundleCollection>
    {
        private string rootPath;

        public CassetteBundleConfiguration(IRootPathProvider rootPathProvider)
        {
            this.rootPath = rootPathProvider.GetRootPath();
        }

        public void Configure(BundleCollection bundles)
        {
            // Please read http://getcassette.net/documentation/configuration
            bundles.Add<StylesheetBundle>("assets/stylesheets/style.less");

            var sprockets = new Sprockets.Sprockets();

            string headerJs = Path.Combine(this.rootPath, "assets/javascripts/header.js");
            var node = sprockets.Scan(headerJs);
            var deps = node.ResolveDependencies();
            bundles.Add<ScriptBundle>(headerJs, deps.Select(d => d.Data.AbsolutePath), b => b.PageLocation = "header");

            bundles.Add<ScriptBundle>("assets/javascripts/_vendors/jquery.js");

            string mainJs = Path.Combine(this.rootPath, "assets/javascripts/main.js");
            node = sprockets.Scan(mainJs);
            deps = node.ResolveDependencies();
            bundles.Add<ScriptBundle>(mainJs, deps.Select(d => d.Data.AbsolutePath), b => b.PageLocation = "app");
        }
    }
}