using System.Web.Mvc;
using ZBuildLights.Core.Services;
using ZBuildLights.Web.Models.Home;

namespace ZBuildLights.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISystemStatusProvider _systemStatusProvider;
        private readonly ILightDisplayUpdater _lightDisplayUpdater;

        public HomeController(ISystemStatusProvider systemStatusProvider, ILightDisplayUpdater lightDisplayUpdater)
        {
            _systemStatusProvider = systemStatusProvider;
            _lightDisplayUpdater = lightDisplayUpdater;
        }

        public ActionResult Index()
        {
            var masterModel = _systemStatusProvider.GetSystemStatus();
            var projects = masterModel.Projects;
            var model = new ViewModel {Projects = projects};
            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult UpdateLights()
        {
            _lightDisplayUpdater.Update();
            return View();
        }
    }
}