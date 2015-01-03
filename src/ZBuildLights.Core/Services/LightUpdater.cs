using System;
using System.Linq;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Repository;

namespace ZBuildLights.Core.Services
{
    public class LightUpdater : ILightUpdater
    {
        private readonly IMasterModelRepository _masterModelRepository;
        private readonly IUnassignedLightService _unassignedLightService;

        public LightUpdater(IMasterModelRepository masterModelRepository, IUnassignedLightService unassignedLightService)
        {
            _masterModelRepository = masterModelRepository;
            _unassignedLightService = unassignedLightService;
        }

        public void Update(uint homeId, byte deviceId, Guid groupId, int colorId)
        {
            var masterModel = _masterModelRepository.GetCurrent();

            var light = FindLight(homeId, deviceId, masterModel);
            light.Color = LightColor.FromValue(colorId);
            light.MoveTo(masterModel.FindGroup(groupId));
            _masterModelRepository.Save(masterModel);
        }

        private Light FindLight(uint homeId, byte deviceId, MasterModel masterModel)
        {
            var allLights = masterModel.AllLights.Union(_unassignedLightService.GetUnassignedLights().Lights);

            var light = allLights
                .SingleOrDefault(x => x.ZWaveHomeId.Equals(homeId) && x.ZWaveDeviceId.Equals(deviceId));
            if (light == null)
                throw new InvalidOperationException(
                    string.Format("Could not find light with homeId: {0} and deviceId: {1}", homeId, deviceId));
            return light;
        }
    }
}