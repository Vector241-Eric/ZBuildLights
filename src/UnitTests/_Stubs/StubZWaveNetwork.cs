using System.Collections.Generic;
using System.Linq;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Services;
using ZBuildLights.Core.Services.Results;

namespace UnitTests._Stubs
{
    public class StubZWaveNetwork : IZWaveNetwork
    {
        private static readonly Dictionary<ZWaveIdentity, SwitchState> _switches =
            new Dictionary<ZWaveIdentity, SwitchState>();

        public ZWaveSwitch[] GetAllSwitches()
        {
            return _switches.Keys.Select(k => new ZWaveSwitch(k) {SwitchState = _switches[k]}).ToArray();
        }

        public ZWaveOperationResult SetSwitchState(ZWaveIdentity identity, SwitchState state)
        {
            _switches[identity] = state;
            return ZWaveOperationResult.Success;
        }
    }
}