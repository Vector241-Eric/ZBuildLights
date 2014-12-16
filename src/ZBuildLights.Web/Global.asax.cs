using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ZBuildLights.Core.Services;

namespace ZBuildLights.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            AutoMapperConfig.Initialize();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        private void InitializeStaticFactories()
        {
            SystemClock.UseCurrentTime();
        }
    }
}