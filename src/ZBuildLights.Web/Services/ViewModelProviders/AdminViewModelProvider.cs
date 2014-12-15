using ZBuildLights.Core.Models;
using ZBuildLights.Core.Services;
using ZBuildLights.Web.Models.Admin;

namespace ZBuildLights.Web.Services.ViewModelProviders
{
    public class AdminViewModelProvider : IAdminViewModelProvider
    {
        private readonly IProjectRepository _projectRepo;
        private readonly ILightAssignmentService _lightAssignmentService;
        private readonly IMapper _mapper;

        public AdminViewModelProvider(IProjectRepository projectRepo, ILightAssignmentService lightAssignmentService, IMapper mapper)
        {
            _projectRepo = projectRepo;
            _lightAssignmentService = lightAssignmentService;
            _mapper = mapper;
        }

        public AdminViewModel GetIndexViewModel()
        {
            var projects = _projectRepo.GetAllProjects();
            var unassigned = _lightAssignmentService.GetUnassignedLights();

            return new AdminViewModel
            {
                Projects = _mapper.Map<Project[], AdminProjectViewModel[]>(projects),
                Unassigned = _mapper.Map<LightGroup, AdminLightGroupViewModel>(unassigned)
            };
        }
    }
}