using ZBuildLights.Core.Models;

namespace ZBuildLights.Core.Repository
{
    public interface IProjectRepository
    {
        Project[] GetAllProjects();
    }
}