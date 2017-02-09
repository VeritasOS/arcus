using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BEArcus.WebApp.Startup))]
namespace BEArcus.WebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
