using ZBuildLights.Core.Models;
using ZBuildLights.Core.Services;

namespace ZBuildLights.Core.Repository
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly IStatusProvider _statusProvider;

        public ProjectRepository(IStatusProvider statusProvider)
        {
            _statusProvider = statusProvider;
        }

        public void CreateProject(string name)
        {
            throw new System.NotImplementedException();
        }

        public Project[] GetAllProjects()
        {
            return _statusProvider.GetCurrentProjects();
        }
    }
}