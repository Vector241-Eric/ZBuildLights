using System;
using System.Net;
using System.Web.Mvc;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Models.Requests;
using ZBuildLights.Core.Services;
using ZBuildLights.Core.Services.CruiseControl;
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
        private readonly ICruiseServerManager _cruiseServerManager;

        public AdminController(IProjectManager projectManager, ILightGroupManager lightGroupManager,
            IAdminViewModelProvider viewModelProvider, ILightUpdater lightUpdater, 
            ICruiseProjectModelProvider ccProjectProvider, ICruiseServerManager cruiseServerManager)
        {
            _projectManager = projectManager;
            _lightGroupManager = lightGroupManager;
            _viewModelProvider = viewModelProvider;
            _lightUpdater = lightUpdater;
            _ccProjectProvider = ccProjectProvider;
            _cruiseServerManager = cruiseServerManager;
        }

        public ActionResult Index()
        {
            var model = _viewModelProvider.GetIndexViewModel();
            return View(model);
        }

        [HttpGet]
        public ActionResult EditProject(Guid? projectId)
        {
            return View(_viewModelProvider.GetEditProjectViewModel(projectId));
        }

        //Projects
        [HttpPost]
        public ActionResult AddProject(string projectName)
        {
            var result = _projectManager.Create(projectName);
            if (result.IsSuccessful)
                return RedirectToAction("Index");
            Response.StatusCode = (int) HttpStatusCode.Conflict;
            return Json(result);
        }

        [HttpPost]
        public ActionResult DeleteProject(Guid projectId)
        {
            _projectManager.Delete(projectId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UpdateProject(Guid projectId, string name)
        {
            var result = _projectManager.Update(projectId, name);
            if (result.IsSuccessful)
                return RedirectToAction("Index");
            Response.StatusCode = (int) HttpStatusCode.Conflict;
            return Json(result);
        }

        //Groups
        [HttpPost]
        public ActionResult AddGroup(Guid projectId, string groupName)
        {
            _lightGroupManager.Create(projectId, groupName);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UpdateGroup(Guid groupId, string name)
        {
            var result = _lightGroupManager.Update(groupId, name);
            if (result.IsSuccessful)
                return RedirectToAction("Index");
            Response.StatusCode = (int) HttpStatusCode.Conflict;
            return Json(result);
        }

        [HttpPost]
        public ActionResult DeleteGroup(Guid groupId)
        {
            _lightGroupManager.Delete(groupId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult EditLight(uint homeId, byte deviceId, Guid groupId, int colorId)
        {
            _lightUpdater.Update(homeId, deviceId, groupId, colorId);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult CcJson(Guid serverId)
        {
            var result = _ccProjectProvider.GetProjects(serverId);
            if (result.IsSuccessful)
            {
                var ccProjectCollection = result;
                return Json(new {Success = true, Projects = ccProjectCollection.Data.Projects}, JsonRequestBehavior.AllowGet);
            }

            throw new Exception("Failed to get projects from cruise server", result.Exception);
        }

        [HttpGet]
        public ActionResult ManageCruiseServers()
        {
            return View(_viewModelProvider.GetCruiseServerViewModels());
        }

        [HttpPost]
        public ActionResult CreateCruiseServer(string name, string url)
        {
            _cruiseServerManager.Create(name, url);
            return RedirectToAction("ManageCruiseServers");
        }
    }
}