using ZBuildLights.Core.Models;
using ZBuildLights.Core.Repository;

namespace ZBuildLights.Core.Services
{
    public class SystemStatusProvider : ISystemStatusProvider
    {
        private readonly IMasterModelRepository _masterModelRepository;
        private readonly ILightStatusSetter _lightStatusSetter;

        public SystemStatusProvider(IMasterModelRepository masterModelRepository, ILightStatusSetter lightStatusSetter)
        {
            _masterModelRepository = masterModelRepository;
            _lightStatusSetter = lightStatusSetter;
        }

        public MasterModel GetSystemStatus()
        {
            var masterModel = _masterModelRepository.GetCurrent();
            _lightStatusSetter.SetLightStatus(masterModel.AllLights);
            return masterModel;
        }
    }
}