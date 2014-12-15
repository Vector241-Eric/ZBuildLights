using System.Web.Mvc;
using ZBuildLights.Core.Services;
using ZBuildLights.Web.Models.Home;

namespace ZBuildLights.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStatusProvider _statusProvider;

        public HomeController(IStatusProvider statusProvider)
        {
            _statusProvider = statusProvider;
        }

        public ActionResult Index()
        {
            var model = new ViewModel {Projects = _statusProvider.GetCurrentProjects()};
            return View(model);
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}