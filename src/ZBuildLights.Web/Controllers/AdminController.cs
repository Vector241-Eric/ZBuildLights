using System;
using System.Net;
using System.Web.Mvc;
using ZBuildLights.Core.Builders;
using ZBuildLights.Web.Services.ViewModelProviders;

namespace ZBuildLights.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly IProjectManager _projectManager;
        private readonly IGroupCreator _groupCreator;
        private readonly IAdminViewModelProvider _viewModelProvider;

        public AdminController(IProjectManager projectManager, IGroupCreator groupCreator,
            IAdminViewModelProvider viewModelProvider)
        {
            _projectManager = projectManager;
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
            var result = _projectManager.CreateProject(projectName);
            if (result.WasSuccessful)
                return RedirectToAction("Index");
            Response.StatusCode = (int) HttpStatusCode.Conflict;
            return Json(result);
        }

        [HttpPost]
        public ActionResult DeleteProject(Guid projectId)
        {
            _projectManager.DeleteProject(projectId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UpdateProject(Guid projectId, string name)
        {
            var result = _projectManager.UpdateProject(projectId, name);
            if (result.WasSuccessful)
                return RedirectToAction("Index");
            Response.StatusCode = (int)HttpStatusCode.Conflict;
            return Json(result);
        }
    }
}