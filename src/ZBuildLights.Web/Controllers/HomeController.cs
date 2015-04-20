using System.Web.Mvc;
using ZBuildLights.Core.Repository;
using ZBuildLights.Core.Services;
using ZBuildLights.Web.Models.Home;

namespace ZBuildLights.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMasterModelRepository _masterModelRepository;
        private readonly ILightDisplayUpdater _lightDisplayUpdater;

        public HomeController(IMasterModelRepository masterModelRepository, ILightDisplayUpdater lightDisplayUpdater)
        {
            _masterModelRepository = masterModelRepository;
            _lightDisplayUpdater = lightDisplayUpdater;
        }

        public ActionResult Index()
        {
            var masterModel = _masterModelRepository.GetCurrent();
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