using ZBuildLights.Core.Models;

namespace ZBuildLights.Core.Services
{
    public interface IStatusProvider
    {
        Project[] GetCurrentProjects();
    }
}