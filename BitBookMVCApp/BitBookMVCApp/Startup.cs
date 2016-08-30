using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BitBookMVCApp.Startup))]
namespace BitBookMVCApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
