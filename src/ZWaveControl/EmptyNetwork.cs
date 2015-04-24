using ZBuildLights.Core.Models;
using ZBuildLights.Core.Services;
using ZBuildLights.Core.Services.Results;

namespace ZWaveControl
{
    public class EmptyNetwork : IZWaveNetwork
    {
        public ZWaveSwitch[] GetAllSwitches()
        {
            return new ZWaveSwitch[0];
        }

        public ZWaveOperationResult SetSwitchState(ZWaveIdentity identity, SwitchState state)
        {
            return ZWaveOperationResult.Fail("ZWave Network is not connected.");
        }
    }
}