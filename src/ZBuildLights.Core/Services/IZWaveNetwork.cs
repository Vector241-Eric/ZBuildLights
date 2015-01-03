using System.Collections.Generic;
using System.Linq;
using ZBuildLights.Core.Models;

namespace ZBuildLights.Core.Services
{
    public interface IZWaveNetwork
    {
        ZWaveSwitch[] GetAllSwitches();
        void SetSwitchState(ZWaveSwitch zwSwitch);
    }

    public class StubZWaveNetwork : IZWaveNetwork
    {
        private static readonly List<ZWaveSwitch> _switches = new List<ZWaveSwitch>
        {
            new ZWaveSwitch {HomeId = 1, DeviceId = 1, SwitchState = SwitchState.Off},
            new ZWaveSwitch {HomeId = 1, DeviceId = 2, SwitchState = SwitchState.Off},
            new ZWaveSwitch {HomeId = 1, DeviceId = 3, SwitchState = SwitchState.Off},
            new ZWaveSwitch {HomeId = 1, DeviceId = 4, SwitchState = SwitchState.Off},
            new ZWaveSwitch {HomeId = 1, DeviceId = 5, SwitchState = SwitchState.Off},
            new ZWaveSwitch {HomeId = 1, DeviceId = 6, SwitchState = SwitchState.Off},
            new ZWaveSwitch {HomeId = 1, DeviceId = 7, SwitchState = SwitchState.Off},
            new ZWaveSwitch {HomeId = 1, DeviceId = 8, SwitchState = SwitchState.Off},
            new ZWaveSwitch {HomeId = 1, DeviceId = 9, SwitchState = SwitchState.Off},
            new ZWaveSwitch {HomeId = 1, DeviceId = 10, SwitchState = SwitchState.Off},
            new ZWaveSwitch {HomeId = 1, DeviceId = 11, SwitchState = SwitchState.Off},
        };

        public ZWaveSwitch[] GetAllSwitches()
        {
            return _switches.ToArray();
        }

        public void SetSwitchState(ZWaveSwitch zwSwitch)
        {
            var zWaveSwitch = _switches.Single(
                x => x.HomeId.Equals(zwSwitch.HomeId)
                     && x.DeviceId.Equals(zwSwitch.DeviceId));
            zWaveSwitch.SwitchState = zwSwitch.SwitchState;
        }
    }
}