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
                Unassigned = new AdminLightGroupViewModel{Name = "Unassigned", Lights = unassignedLights},
            };
        }
    }
}