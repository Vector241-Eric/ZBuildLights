using System;
using System.Net;
using System.Web.Mvc;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Services;
using ZBuildLights.Core.Services.CruiseControl;
using ZBuildLights.Core.Services.Results;
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
        public ActionResult EditProject(Guid? projectId, EditProjectCruiseProject[] projects)
        {
            var editProjectMasterViewModel = _viewModelProvider.GetEditProjectViewModel(projectId);
            if (TempData.ContainsKey("ErrorMessage"))
                editProjectMasterViewModel.ErrorMessage = (string) TempData["ErrorMessage"];
            return View(editProjectMasterViewModel);
        }

        [HttpPost]
        public ActionResult EditProject(EditProject editModel)
        {
            ICrudResult<Project> result;
            if (editModel.ProjectId == null || editModel.ProjectId.Value == Guid.Empty)
                result = _projectManager.Create(editModel);
            else
                result = _projectManager.Update(editModel);
            if (result.IsSuccessful)
                return RedirectToAction("Index");
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction("EditProject", new {projectId = editModel.ProjectId, projects = editModel.CruiseProjects});
        }

        [HttpPost]
        public ActionResult DeleteProject(Guid projectId)
        {
            _projectManager.Delete(projectId);
            return RedirectToAction("Index");
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
        public ActionResult EditLight(uint homeId, byte deviceId, ulong valueId, Guid groupId, int colorId)
        {
            _lightUpdater.Update(new ZWaveIdentity(homeId, deviceId, valueId), groupId, colorId);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult CcJson(Guid serverId)
        {
            var result = _ccProjectProvider.GetProjects(serverId);
            if (result.IsSuccessful)
            {
                var ccProjectCollection = result;
                return Json(new {Success = true, ccProjectCollection.Data.Projects}, JsonRequestBehavior.AllowGet);
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