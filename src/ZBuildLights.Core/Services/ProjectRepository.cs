using ZBuildLights.Core.Models;

namespace ZBuildLights.Core.Services
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