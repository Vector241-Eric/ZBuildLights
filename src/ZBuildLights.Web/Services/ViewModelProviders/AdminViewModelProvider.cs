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
            var systemStatus = _systemStatusProvider.GetSystemStatus();
            var projects = systemStatus.MasterModel.Projects;
            var unassigned = systemStatus.UnassignedLights;

            return new AdminViewModel
            {
                Projects = _mapper.Map<Project[], AdminProjectViewModel[]>(projects),
                Unassigned = _mapper.Map<LightGroup, AdminLightGroupViewModel>(unassigned),
            };
        }
    }
}