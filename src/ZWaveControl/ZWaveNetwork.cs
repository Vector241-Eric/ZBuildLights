using System;
using System.Collections.Generic;
using OpenZWaveDotNet;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Services;

namespace ZWaveControl
{
    public class ZWaveNetwork : IZWaveNetwork
    {
        private ZWManager _manager;

        public ZWaveNetwork()
        {
            _manager = ZWaveManagerFactory.GetInstance();
        }

        public ZWaveSwitch[] GetAllSwitches()
        {
            var nodes = ZWaveNotificationHandler.GetNodes();
            var switches = new List<ZWaveSwitch>();
            foreach (var node in nodes)
            {
                foreach (var value in node.Values)
                {
                    var valueLabel = _manager.GetValueLabel(value);
                    if (valueLabel.ToLowerInvariant().Equals("switch"))
                    {
                        var zWaveSwitch = GetZWaveSwitch(value, node);
                        switches.Add(zWaveSwitch);
                    }
                }
            }
            return switches.ToArray();
        }

        private ZWaveSwitch GetZWaveSwitch(ZWValueID value, Node node)
        {
            bool switchValue;
            var successfullyReadValue = _manager.GetValueAsBool(value, out switchValue);
            var zWaveSwitch = new ZWaveSwitch
            {
                HomeId = node.HomeId,
                DeviceId = node.Id,
                ValueId = value.GetId()
            };
            if (successfullyReadValue)
                zWaveSwitch.SwitchState = switchValue ? SwitchState.On : SwitchState.Off;
            else
                zWaveSwitch.SwitchState = SwitchState.Unknown;
            return zWaveSwitch;
        }

        public void SetSwitchState(ZWaveSwitch zwSwitch)
        {
            throw new NotImplementedException();
        }
    }
}