using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ButlerSite.Startup))]
namespace ButlerSite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
