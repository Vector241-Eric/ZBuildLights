using System.Web.Mvc;
using ZBuildLights.Core.Repository;
using ZBuildLights.Core.Services;
using ZBuildLights.Web.Services.ViewModelProviders;

namespace ZBuildLights.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IAdminViewModelProvider _viewModelProvider;

        public AdminController(IProjectRepository projectRepository, IGroupRepository groupRepository,
            IAdminViewModelProvider viewModelProvider)
        {
            _projectRepository = projectRepository;
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
            _projectRepository.CreateProject(projectName);
            return Json(new {Success = true});
        }
    }
}