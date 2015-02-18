using System;
using System.Collections.Generic;
using System.Linq;
using ZBuildLights.Core.Extensions;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Services;
using ZBuildLights.Core.Wrappers;
using ZBuildLights.Web.Models.Admin;

namespace ZBuildLights.Web.Services.ViewModelProviders
{
    public class AdminViewModelProvider : IAdminViewModelProvider
    {
        private readonly ISystemStatusProvider _systemStatusProvider;
        private readonly IMapper _mapper;

        public AdminViewModelProvider(ISystemStatusProvider systemStatusProvider,
            IMapper mapper)
        {
            _systemStatusProvider = systemStatusProvider;
            _mapper = mapper;
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

        public EditProjectViewModel GetEditProjectViewModel(Guid? id)
        {
            var masterModel = _systemStatusProvider.GetSystemStatus();

            Project project;
            if (id == null)
                project = masterModel.CreateProject();
            else
            {
                if (masterModel.Projects.None(x => x.Id.Equals(id.Value)))
                    throw new ArgumentException("Could not find a project with the requested ID.");
                project = masterModel.Projects.SingleOrDefault(x => x.Id == id.Value);
            }

            var mappedProject = _mapper.Map<Project, AdminProjectViewModel>(project);
            var mappedCruiseServers =
                _mapper.Map<CruiseServer[], EditCruiseServerViewModel[]>(masterModel.CruiseServers);
            return new EditProjectViewModel
            {
                Project = mappedProject,
                CruiseServers = mappedCruiseServers
            };
        }

        public EditCruiseServerViewModel[] GetCruiseServerViewModels()
        {
            var masterModel = _systemStatusProvider.GetSystemStatus();
            return _mapper.Map<CruiseServer[], EditCruiseServerViewModel[]>(masterModel.CruiseServers);
        }
    }
}