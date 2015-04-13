using System.Collections.Generic;
using System.Linq;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Services.Results;

namespace ZBuildLights.Core.Services
{
    public interface IZWaveNetwork
    {
        ZWaveSwitch[] GetAllSwitches();
        ZWaveOperationResult SetSwitchState(ZWaveSwitch zwSwitch);
    }

    public class StubZWaveNetwork : IZWaveNetwork
    {
        private static readonly List<ZWaveSwitch> _switches = new List<ZWaveSwitch>
        {
            new ZWaveSwitch(new ZWaveIdentity(1, 1, 1)) {SwitchState = SwitchState.Off},
            new ZWaveSwitch(new ZWaveIdentity(1, 2, 1)) {SwitchState = SwitchState.Off},
            new ZWaveSwitch(new ZWaveIdentity(1, 3, 1)) {SwitchState = SwitchState.Off},
            new ZWaveSwitch(new ZWaveIdentity(1, 4, 1)) {SwitchState = SwitchState.Off},
            new ZWaveSwitch(new ZWaveIdentity(1, 5, 1)) {SwitchState = SwitchState.Off},
            new ZWaveSwitch(new ZWaveIdentity(1, 6, 1)) {SwitchState = SwitchState.Off},
            new ZWaveSwitch(new ZWaveIdentity(1, 7, 1)) {SwitchState = SwitchState.Off},
            new ZWaveSwitch(new ZWaveIdentity(1, 8, 1)) {SwitchState = SwitchState.Off},
            new ZWaveSwitch(new ZWaveIdentity(1, 9, 1)) {SwitchState = SwitchState.Off},
            new ZWaveSwitch(new ZWaveIdentity(1, 10, 1)) {SwitchState = SwitchState.Off},
            new ZWaveSwitch(new ZWaveIdentity(1, 11, 1)) {SwitchState = SwitchState.Off}
        };

        public ZWaveSwitch[] GetAllSwitches()
        {
            return _switches.ToArray();
        }

        public ZWaveOperationResult SetSwitchState(ZWaveSwitch zwSwitch)
        {
            var zWaveSwitch = _switches.Single(x => x.ZWaveIdentity.Equals(zwSwitch.ZWaveIdentity));
            zWaveSwitch.SwitchState = zwSwitch.SwitchState;
            return ZWaveOperationResult.Success;
        }
    }
}