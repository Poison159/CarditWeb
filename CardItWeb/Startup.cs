using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CardItWeb.Startup))]
namespace CardItWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
