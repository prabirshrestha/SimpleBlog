using Cassette;
using Cassette.Scripts;
using Cassette.Stylesheets;

namespace SimpleBlog
{
    /// <summary>
    /// Configures the Cassette asset bundles for the web application.
    /// </summary>
    public class CassetteBundleConfiguration : IConfiguration<BundleCollection>
    {
        public void Configure(BundleCollection bundles)
        {
            // Please read http://getcassette.net/documentation/configuration
        }
    }
}