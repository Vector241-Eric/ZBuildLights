using ZBuildLights.Core.Models;
using ZBuildLights.Core.Repository;

namespace ZBuildLights.Core.Services
{
    public class SystemStatusProvider : ISystemStatusProvider
    {
        private readonly IMasterModelRepository _masterModelRepository;
        private readonly ILightStatusSetter _lightStatusSetter;
        private readonly IUnassignedLightService _unassignedLightService;

        public SystemStatusProvider(IMasterModelRepository masterModelRepository, ILightStatusSetter lightStatusSetter,
            IUnassignedLightService unassignedLightService)
        {
            _masterModelRepository = masterModelRepository;
            _lightStatusSetter = lightStatusSetter;
            _unassignedLightService = unassignedLightService;
        }

        public SystemStatusModel GetSystemStatus()
        {
            var unassignedGroup = _unassignedLightService.GetUnassignedLights();
            var masterModel = _masterModelRepository.GetCurrent();
            _lightStatusSetter.SetLightStatus(masterModel.AllLights);
            return new SystemStatusModel {MasterModel = masterModel, UnassignedLights = unassignedGroup};
        }
    }
}