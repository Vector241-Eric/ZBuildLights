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
            new ZWaveSwitch {HomeId = 1, NodeId = 1, SwitchState = SwitchState.Off},
            new ZWaveSwitch {HomeId = 1, NodeId = 2, SwitchState = SwitchState.Off},
            new ZWaveSwitch {HomeId = 1, NodeId = 3, SwitchState = SwitchState.Off},
            new ZWaveSwitch {HomeId = 1, NodeId = 4, SwitchState = SwitchState.Off},
            new ZWaveSwitch {HomeId = 1, NodeId = 5, SwitchState = SwitchState.Off},
            new ZWaveSwitch {HomeId = 1, NodeId = 6, SwitchState = SwitchState.Off},
            new ZWaveSwitch {HomeId = 1, NodeId = 7, SwitchState = SwitchState.Off},
            new ZWaveSwitch {HomeId = 1, NodeId = 8, SwitchState = SwitchState.Off},
            new ZWaveSwitch {HomeId = 1, NodeId = 9, SwitchState = SwitchState.Off},
            new ZWaveSwitch {HomeId = 1, NodeId = 10, SwitchState = SwitchState.Off},
            new ZWaveSwitch {HomeId = 1, NodeId = 11, SwitchState = SwitchState.Off},
        };

        public ZWaveSwitch[] GetAllSwitches()
        {
            return _switches.ToArray();
        }

        public ZWaveOperationResult SetSwitchState(ZWaveSwitch zwSwitch)
        {
            var zWaveSwitch = _switches.Single(
                x => x.HomeId.Equals(zwSwitch.HomeId)
                     && x.NodeId.Equals(zwSwitch.NodeId));
            zWaveSwitch.SwitchState = zwSwitch.SwitchState;
            return ZWaveOperationResult.Success;
        }
    }
}