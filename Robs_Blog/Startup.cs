using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Robs_Blog.Startup))]
namespace Robs_Blog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
