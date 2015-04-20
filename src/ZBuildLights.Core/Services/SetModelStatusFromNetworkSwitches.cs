using System.Collections.Generic;
using System.Linq;
using ZBuildLights.Core.Models;

namespace ZBuildLights.Core.Services
{
    public class SetModelStatusFromNetworkSwitches : ISetModelStatusFromNetworkSwitches
    {
        private readonly IZWaveNetwork _network;

        public SetModelStatusFromNetworkSwitches(IZWaveNetwork network)
        {
            _network = network;
        }

        public void SetLightStatus(IEnumerable<Light> lights)
        {
            var zWaveSwitches = _network.GetAllSwitches();
            foreach (var light in lights)
            {
                var zwSwitch = zWaveSwitches.SingleOrDefault(x => x.ZWaveIdentity.Equals(light.ZWaveIdentity));
                if (zwSwitch != null)
                    light.SwitchState = zwSwitch.SwitchState;
                else
                    light.SwitchState = SwitchState.Unknown;
            }
        }
    }
}