using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using NLog;
using ZBuildLights.Core.Services;

namespace ZBuildLights.Web
{
    public class MvcApplication : HttpApplication
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private static readonly Guid ApplicationId = Guid.NewGuid();

        protected void Application_Start()
        {
            Log.Debug("Application ({0}) starting", ApplicationId);

            AreaRegistration.RegisterAllAreas();
            InitializeStaticFactories();
            AutoMapperConfig.Initialize();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        private void InitializeStaticFactories()
        {
            SystemClock.UseCurrentTime();
        }

        protected void Application_End()
        {
            Log.Debug("Application ({0}) ending", ApplicationId);
        }
    }
}