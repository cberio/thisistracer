using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using thisistracer.App_Start;

namespace thisistracer
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundle(BundleTable.Bundles);
        }
    }
}
