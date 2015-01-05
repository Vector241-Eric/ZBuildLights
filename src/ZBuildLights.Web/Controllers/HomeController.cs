using System.Web.Mvc;
using ZBuildLights.Core.Repository;
using ZBuildLights.Web.Models.Home;

namespace ZBuildLights.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMasterModelRepository _masterModelRepository;

        public HomeController(IMasterModelRepository masterModelRepository)
        {
            _masterModelRepository = masterModelRepository;
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

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}