using System.Collections.Generic;
using ZBuildLights.Core.Models;

namespace ZBuildLights.Core.Services
{
    public interface ISetModelStatusFromNetworkSwitches
    {
        void SetLightStatus(IEnumerable<Light> lights);
    }
}