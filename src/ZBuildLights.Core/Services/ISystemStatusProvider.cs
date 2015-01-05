using ZBuildLights.Core.Models;

namespace ZBuildLights.Core.Services
{
    public interface ISystemStatusProvider
    {
        MasterModel GetSystemStatus();
    }
}