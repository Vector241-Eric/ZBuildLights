using ZBuildLights.Core.Models;

namespace ZBuildLights.Core.Services
{
    public interface IProjectStatusUpdater
    {
        void UpdateAllProjectStatuses(MasterModel masterModel);
    }
}