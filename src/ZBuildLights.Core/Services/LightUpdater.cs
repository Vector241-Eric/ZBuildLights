using System;
using System.Linq;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Repository;

namespace ZBuildLights.Core.Services
{
    public class LightUpdater : ILightUpdater
    {
        private readonly IMasterModelRepository _masterModelRepository;

        public LightUpdater(IMasterModelRepository masterModelRepository)
        {
            _masterModelRepository = masterModelRepository;
        }

        public void Update(uint homeId, byte deviceId, Guid groupId, int colorId)
        {
            var masterModel = _masterModelRepository.GetCurrent();
            var light = FindLight(homeId, deviceId, masterModel);

            //Set color
            light.Color = LightColor.FromValue(colorId);
            
            //Set group
            if (groupId == Guid.Empty)
            {
                light.Unassign();
                masterModel.AddUnassignedLight(light);
            }
            else
            {
                masterModel.AssignLightToGroup(light.ZWaveHomeId, light.ZWaveDeviceId, groupId);
            }

            //Save
            _masterModelRepository.Save(masterModel);
        }

        private Light FindLight(uint homeId, byte deviceId, MasterModel masterModel)
        {
            var light = masterModel.AllLights
                .SingleOrDefault(x => x.ZWaveHomeId.Equals(homeId) && x.ZWaveDeviceId.Equals(deviceId));
            if (light == null)
                throw new InvalidOperationException(
                    string.Format("Could not find light with homeId: {0} and deviceId: {1}", homeId, deviceId));
            return light;
        }
    }
}