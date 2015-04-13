using System.Collections.Generic;
using System.Linq;
using OpenZWaveDotNet;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Services;
using ZBuildLights.Core.Services.Results;

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
            var zWaveSwitch = new ZWaveSwitch(new ZWaveIdentity(node.HomeId, node.Id, value.GetId()));
            if (successfullyReadValue)
                zWaveSwitch.SwitchState = switchValue ? SwitchState.On : SwitchState.Off;
            else
                zWaveSwitch.SwitchState = SwitchState.Unknown;
            return zWaveSwitch;
        }

        public ZWaveOperationResult SetSwitchState(ZWaveSwitch zwSwitch)
        {
            var node =
                ZWaveNotificationHandler.GetNodes()
                    .SingleOrDefault(x => x.HomeId.Equals(zwSwitch.ZWaveIdentity.HomeId) && x.Id.Equals(zwSwitch.ZWaveIdentity.NodeId));
            if (node == null)
            {
                var message = string.Format("Could not locate a node with HomeId {0} and NodeId {1}",
                    zwSwitch.ZWaveIdentity.HomeId, zwSwitch.ZWaveIdentity.NodeId);
                return ZWaveOperationResult.Fail(message);
            }

            var value = node.Values.SingleOrDefault(x => x.GetId().Equals(zwSwitch.ZWaveIdentity.ValueId));
            if (value == null)
            {
                var message = string.Format("Could not locate a value with HomeId {0} NodeId {1} and ValueId {2}",
                    zwSwitch.ZWaveIdentity.HomeId, zwSwitch.ZWaveIdentity.NodeId, zwSwitch.ZWaveIdentity.ValueId);
                return ZWaveOperationResult.Fail(message);
            }

            var switchValueBool = zwSwitch.SwitchState == SwitchState.On;
            _manager.SetValue(value, switchValueBool);
            return ZWaveOperationResult.Success;
        }
    }
}