using ZBuildLights.Core.Models;
using ZBuildLights.Core.Repository;

namespace ZBuildLights.Core.Services
{
    public class SystemStatusProvider : ISystemStatusProvider
    {
        private readonly IMasterModelRepository _masterModelRepository;
        private readonly ISetModelStatusFromNetworkSwitches _setModelStatusFromNetworkSwitches;

        public SystemStatusProvider(IMasterModelRepository masterModelRepository, ISetModelStatusFromNetworkSwitches setModelStatusFromNetworkSwitches)
        {
            _masterModelRepository = masterModelRepository;
            _setModelStatusFromNetworkSwitches = setModelStatusFromNetworkSwitches;
        }

        public MasterModel GetSystemStatus()
        {
            var masterModel = _masterModelRepository.GetCurrent();
            _setModelStatusFromNetworkSwitches.SetLightStatus(masterModel.AllLights);
            return masterModel;
        }
    }
}