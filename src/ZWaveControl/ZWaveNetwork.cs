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
        private readonly IZWaveNodeList _nodeList;
        private ZWManager _manager;

        public ZWaveNetwork(IZWaveNodeList nodeList, IZWaveManagerFactory managerFactory)
        {
            _nodeList = nodeList;
            _manager = managerFactory.GetManager();
        }

        public ZWaveSwitch[] GetAllSwitches()
        {
            var nodes = _nodeList.AllNodes;
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
            var zWaveSwitch = new ZWaveSwitch(new ZWaveValueIdentity(node.HomeId, node.NodeId, value.GetId()));
            if (successfullyReadValue)
                zWaveSwitch.SwitchState = switchValue ? SwitchState.On : SwitchState.Off;
            else
                zWaveSwitch.SwitchState = SwitchState.Unknown;
            return zWaveSwitch;
        }

        public ZWaveOperationResult SetSwitchState(ZWaveValueIdentity identity, SwitchState state)
        {
            var node = _nodeList.AllNodes.SingleOrDefault(x => x.NodeIdentity.Equals(identity.NodeIdentity));
            if (node == null)
            {
                var message = string.Format("Could not locate a node with HomeId {0} and NodeId {1}",
                    identity.HomeId, identity.NodeId);
                return ZWaveOperationResult.Fail(message);
            }

            var value = node.Values.SingleOrDefault(x => x.GetId().Equals(identity.ValueId));
            if (value == null)
            {
                var message = string.Format("Could not locate a value for {0}", identity);
                return ZWaveOperationResult.Fail(message);
            }

            var switchValueBool = state == SwitchState.On;
            _manager.SetValue(value, switchValueBool);
            return ZWaveOperationResult.Success;
        }
    }
}