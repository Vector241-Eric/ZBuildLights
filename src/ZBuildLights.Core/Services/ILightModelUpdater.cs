using System;
using ZBuildLights.Core.Models;

namespace ZBuildLights.Core.Services
{
    public interface ILightModelUpdater
    {
        void Update(ZWaveValueIdentity identity, Guid groupId, int colorId);
    }
}