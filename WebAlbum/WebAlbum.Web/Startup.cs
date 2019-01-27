using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebAlbum.Web.Startup))]
namespace WebAlbum.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
