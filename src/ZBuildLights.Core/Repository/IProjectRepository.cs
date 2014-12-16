using ZBuildLights.Core.Models;

namespace ZBuildLights.Core.Repository
{
    public interface IProjectRepository
    {
        void CreateProject(string name);
        Project[] GetAllProjects();
    }
}