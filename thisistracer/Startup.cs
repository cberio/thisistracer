using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(thisistracer.Startup))]
namespace thisistracer 
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
