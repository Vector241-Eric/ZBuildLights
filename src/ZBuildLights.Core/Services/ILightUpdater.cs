using System;

namespace ZBuildLights.Core.Services
{
    public interface ILightUpdater
    {
        void Update(uint homeId, byte deviceId, Guid groupId, int colorId);
    }
}