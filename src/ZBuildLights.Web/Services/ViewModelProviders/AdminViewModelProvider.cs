using ZBuildLights.Core.Models;
using ZBuildLights.Core.Repository;
using ZBuildLights.Core.Services;
using ZBuildLights.Core.Wrappers;
using ZBuildLights.Web.Models.Admin;

namespace ZBuildLights.Web.Services.ViewModelProviders
{
    public class AdminViewModelProvider : IAdminViewModelProvider
    {
        private readonly IMasterModelRepository _masterModelRepository;
        private readonly ILightAssignmentService _lightAssignmentService;
        private readonly IMapper _mapper;

        public AdminViewModelProvider(IMasterModelRepository masterModelRepository, ILightAssignmentService lightAssignmentService,
            IMapper mapper)
        {
            _masterModelRepository = masterModelRepository;
            _lightAssignmentService = lightAssignmentService;
            _mapper = mapper;
        }

        public AdminViewModel GetIndexViewModel()
        {
            var projects = _masterModelRepository.GetCurrent().Projects;
            var unassigned = _lightAssignmentService.GetUnassignedLights();

            return new AdminViewModel
            {
                Projects = _mapper.Map<Project[], AdminProjectViewModel[]>(projects),
                Unassigned = _mapper.Map<LightGroup, AdminLightGroupViewModel>(unassigned)
            };
        }
    }
}