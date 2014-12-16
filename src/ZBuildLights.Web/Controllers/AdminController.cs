using System.Web.Mvc;
using ZBuildLights.Core.Builders;
using ZBuildLights.Core.Repository;
using ZBuildLights.Web.Services.ViewModelProviders;

namespace ZBuildLights.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly IProjectCreator _projectCreator;
        private readonly IGroupCreator _groupCreator;
        private readonly IAdminViewModelProvider _viewModelProvider;

        public AdminController(IProjectCreator projectCreator, IGroupCreator groupCreator,
            IAdminViewModelProvider viewModelProvider)
        {
            _projectCreator = projectCreator;
            _groupCreator = groupCreator;
            _viewModelProvider = viewModelProvider;
        }

        public ActionResult Index()
        {
            var model = _viewModelProvider.GetIndexViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult AddGroup(string groupName)
        {
            _groupCreator.CreateGroup(groupName);
            return Json(new {Success = true});
        }

        [HttpPost]
        public ActionResult AddProject(string projectName)
        {
            _projectCreator.CreateProject(projectName);
            return RedirectToAction("Index");
//            return Json(new {Success = true});
        }
    }
}