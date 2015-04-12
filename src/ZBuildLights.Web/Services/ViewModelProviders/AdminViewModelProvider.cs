using System;
using System.Linq;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Models.CruiseControl;
using ZBuildLights.Core.Services;
using ZBuildLights.Core.Services.CruiseControl;
using ZBuildLights.Core.Wrappers;
using ZBuildLights.Web.Models.Admin;

namespace ZBuildLights.Web.Services.ViewModelProviders
{
    public class AdminViewModelProvider : IAdminViewModelProvider
    {
        private readonly ISystemStatusProvider _systemStatusProvider;
        private readonly IMapper _mapper;
        private readonly ICruiseProjectModelProvider _cruiseProjectProvider;

        public AdminViewModelProvider(ISystemStatusProvider systemStatusProvider,
            IMapper mapper, ICruiseProjectModelProvider cruiseProjectProvider)
        {
            _systemStatusProvider = systemStatusProvider;
            _mapper = mapper;
            _cruiseProjectProvider = cruiseProjectProvider;
        }

        public AdminViewModel GetIndexViewModel()
        {
            var masterModel = _systemStatusProvider.GetSystemStatus();

            var unassignedLights = _mapper.Map<Light[], AdminLightViewModel[]>(masterModel.UnassignedLights);
            var projectViewModels = _mapper.Map<Project[], AdminProjectViewModel[]>(masterModel.Projects);

            return new AdminViewModel
            {
                Projects = projectViewModels,
                Unassigned = new AdminLightGroupViewModel {Name = "Unassigned", Lights = unassignedLights}
            };
        }

        public EditCruiseServerViewModel[] GetCruiseServerViewModels()
        {
            var masterModel = _systemStatusProvider.GetSystemStatus();
            return _mapper.Map<CruiseServer[], EditCruiseServerViewModel[]>(masterModel.CruiseServers);
        }

        public EditProjectMasterViewModel GetEditProjectViewModel(Guid? id)
        {
            var status = _systemStatusProvider.GetSystemStatus();
            EditProjectViewModel projectViewModel;
            if (id == null)
                projectViewModel = new EditProjectViewModel();
            else
            {
                var project = status.Projects.SingleOrDefault(x => x.Id.Equals(id.Value));
                if (project == null)
                    throw new ArgumentException("ID does not match any existing projects", "id");
                projectViewModel = _mapper.Map<Project, EditProjectViewModel>(project);
            }
            return new EditProjectMasterViewModel
            {
                Project = projectViewModel,
                CruiseServers = GetServerViewModels(status),
                ShowDelete = id != null,
                HeaderText = id == null ? "Create Project" : "Edit Project"
            };
        }

        private EditProjectCruiseServerViewModel[] GetServerViewModels(MasterModel status)
        {
            var cruiseServerViewModels =
                _mapper.Map<CruiseServer[], EditProjectCruiseServerViewModel[]>(status.CruiseServers);
            foreach (var server in cruiseServerViewModels)
            {
                var networkResponse = _cruiseProjectProvider.GetProjects(server.Id);
                var projects = networkResponse.IsSuccessful
                    ? networkResponse.Data.Projects
                    : new CcProjectViewModel[0];
                server.Projects = projects;
            }
            return cruiseServerViewModels;
        }
    }
}