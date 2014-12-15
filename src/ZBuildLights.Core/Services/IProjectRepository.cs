using ZBuildLights.Core.Models;

namespace ZBuildLights.Core.Services
{
    public interface IProjectRepository
    {
        void CreateProject(string name);
        Project[] GetAllProjects();
    }
}