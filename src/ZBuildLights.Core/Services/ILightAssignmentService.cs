using ZBuildLights.Core.Models;

namespace ZBuildLights.Core.Services
{
    public interface ILightAssignmentService
    {
        LightGroup GetUnassignedLights();
    }
}