using ZBuildLights.Core.Models;

namespace ZBuildLights.Core.Services
{
    public interface IUnassignedLightService
    {
        void SetUnassignedLights(MasterModel masterModel);
    }
}