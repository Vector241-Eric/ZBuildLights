using System;
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
            var light = masterModel.FindLight(homeId, deviceId);
            light.Color = LightColor.FromValue(colorId);
            light.MoveTo(masterModel.FindGroup(groupId));
            _masterModelRepository.Save(masterModel);
        }
    }
}