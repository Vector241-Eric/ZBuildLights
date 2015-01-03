using ZBuildLights.Core.Models;

namespace ZBuildLights.Core.Services
{
    public interface IUnassignedLightService
    {
        LightGroup GetUnassignedLights();
    }
}