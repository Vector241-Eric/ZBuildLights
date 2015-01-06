using System;
using System.Net;
using System.Web.Mvc;
using ZBuildLights.Core.Services;
using ZBuildLights.Web.Services.ViewModelProviders;

namespace ZBuildLights.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly IProjectManager _projectManager;
        private readonly ILightGroupManager _lightGroupManager;
        private readonly IAdminViewModelProvider _viewModelProvider;
        private readonly ILightUpdater _lightUpdater;
        private readonly ICruiseProjectModelProvider _ccProjectProvider;

        public AdminController(IProjectManager projectManager, ILightGroupManager lightGroupManager,
            IAdminViewModelProvider viewModelProvider, ILightUpdater lightUpdater, ICruiseProjectModelProvider ccProjectProvider)
        {
            _projectManager = projectManager;
            _lightGroupManager = lightGroupManager;
            _viewModelProvider = viewModelProvider;
            _lightUpdater = lightUpdater;
            _ccProjectProvider = ccProjectProvider;
        }

        public ActionResult Index()
        {
            var model = _viewModelProvider.GetIndexViewModel();
            return View(model);
        }

        //Projects
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
            Response.StatusCode = (int) HttpStatusCode.Conflict;
            return Json(result);
        }

        //Groups
        [HttpPost]
        public ActionResult AddGroup(Guid projectId, string groupName)
        {
            _lightGroupManager.CreateLightGroup(projectId, groupName);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UpdateGroup(Guid groupId, string name)
        {
            var result = _lightGroupManager.UpdateLightGroup(groupId, name);
            if (result.WasSuccessful)
                return RedirectToAction("Index");
            Response.StatusCode = (int) HttpStatusCode.Conflict;
            return Json(result);
        }

        [HttpPost]
        public ActionResult DeleteGroup(Guid groupId)
        {
            _lightGroupManager.DeleteLightGroup(groupId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult EditLight(uint homeId, byte deviceId, Guid groupId, int colorId)
        {
            _lightUpdater.Update(homeId, deviceId, groupId, colorId);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult CcJson(string url)
        {
            var projects = _ccProjectProvider.GetProjects(url);
            return Json(new {Success = true, Projects = projects}, JsonRequestBehavior.AllowGet);
        }
    }
}