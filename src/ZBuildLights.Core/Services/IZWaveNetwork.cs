using ZBuildLights.Core.Models;
using ZBuildLights.Core.Services.Results;

namespace ZBuildLights.Core.Services
{
    public interface IZWaveNetwork
    {
        ZWaveSwitch[] GetAllSwitches();
        ZWaveOperationResult SetSwitchState(ZWaveValueIdentity identity, SwitchState state);
    }
}