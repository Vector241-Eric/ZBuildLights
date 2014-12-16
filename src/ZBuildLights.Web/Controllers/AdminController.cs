using System.Web.Mvc;
using ZBuildLights.Core.Builders;
using ZBuildLights.Core.Repository;
using ZBuildLights.Web.Services.ViewModelProviders;

namespace ZBuildLights.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly IProjectCreator _projectCreator;
        private readonly IGroupRepository _groupRepository;
        private readonly IAdminViewModelProvider _viewModelProvider;

        public AdminController(IProjectCreator projectCreator, IGroupRepository groupRepository,
            IAdminViewModelProvider viewModelProvider)
        {
            _projectCreator = projectCreator;
            _groupRepository = groupRepository;
            _viewModelProvider = viewModelProvider;
        }

        public ActionResult Index()
        {
            return View(_viewModelProvider.GetIndexViewModel());
        }

        [HttpPost]
        public ActionResult AddGroup(string groupName)
        {
            _groupRepository.CreateGroup(groupName);
            return Json(new {Success = true});
        }

        [HttpPost]
        public ActionResult AddProject(string projectName)
        {
            _projectCreator.CreateProject(projectName);
            return Json(new {Success = true});
        }
    }
}