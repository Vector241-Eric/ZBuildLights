using ZBuildLights.Core.Models;

namespace ZBuildLights.Core.Services
{
    public interface IProjectProvider
    {
        Project[] GetCurrentProjects();
    }
}